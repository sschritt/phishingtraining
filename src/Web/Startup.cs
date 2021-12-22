using System;
using System.IO;
using System.Reflection;
using Autofac;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Filters;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Helpers.Security;
using PhishingTraining.Web.Jobs;
using Younited.HealthCheck.EntityFrameworkMigrations;

namespace PhishingTraining.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"/var/dotnet/dataprotection"));
            //TODO .ProtectKeysWithCertificate("thumbprint");

            var defaultConnectionBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"))
            {
                Password = Configuration.ConnectionStringDefaultConnectionPassword()
            };
            var hangfireConnectionBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("HangfireConnection"))
            {
                Password = Configuration.ConnectionStringDefaultConnectionPassword()
            };

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(defaultConnectionBuilder.ConnectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Password.RequiredUniqueChars = 10;
                    options.Password.RequiredLength = 12;
                    options.Password.RequireNonAlphanumeric = options.Password.RequireUppercase = false;
                }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddPwnedPasswordValidator<ApplicationUser>()
                .AddSbaLocalization();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.AddAuthorizationPolicies();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddControllersWithViews(options =>
                {
                    options.Filters.Add<SaveChangesFilter>();
                    options.Filters.Add<RedirectToJoinCampaignFilter>();
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResources));
                });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            if (Environment.IsDevelopment())
            {
                services.AddControllersWithViews().AddRazorRuntimeCompilation();
            }

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>()
                .AddEntityFrameworkMigrationsCheck<ApplicationDbContext>()
                .AddSmtpHealthCheck(options =>
                {
                    options.Host = Configuration.MailServerHost();
                    options.Port = Convert.ToInt32(Configuration.MailServerPort());
                });

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnectionBuilder.ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();

            services.AddHostedService<JobScheduler>();
            services.AddSbaTagHelpers();
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<EmailSender>().AsImplementedInterfaces();
            //builder.RegisterType<ApplicationEmailSender>().AsImplementedInterfaces();
            //builder.RegisterType<ApplicationSmsSender>().AsImplementedInterfaces();
            //builder.RegisterType<MessageGenerator>().AsImplementedInterfaces();
            //builder.RegisterType<ReportService>().AsImplementedInterfaces();
            //TODO Remove above lines when things work (2021-02-25)
            //Register by Convention - all classes with its interfaces in "PhishingTraining.Web.Services" 
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(x => x.Namespace == "PhishingTraining.Web.Services")
                .AsImplementedInterfaces();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders();

            var supportedCultures = new[] { "en", "de" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            loggerFactory.AddFile(Configuration.GetSection("Logging"));
            loggerFactory.AddFile(Configuration.GetSection("ErrorLogging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            #region nwebsec middleware
            app.UseRedirectValidation();
            app.UseXRobotsTag(options => options.NoIndex().NoFollow());
            app.UseXContentTypeOptions();
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.SameOrigin());
            app.UseReferrerPolicy(opts => opts.StrictOriginWhenCrossOrigin());
            app.UseCsp(options =>
            {
                options.DefaultSources(s => s.Self().CustomSources("data:"));
                options.StyleSources(s => s.Self().UnsafeInline());
                options.ScriptSources(s => s.Self());
            });
            #endregion
            //Feature-Policy (https://www.hanselman.com/blog/easily-adding-security-headers-to-your-aspnet-core-web-app-and-getting-an-a-grade)
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Permissions-Policy", "geolocation=(), midi=(), notifications=(), push=(), sync-xhr=(), microphone=(), camera=(), magnetometer=(), gyroscope=(), speaker=(self), vibrate=(), fullscreen=(self), payment=()");
                await next.Invoke();
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health").RequireHost("localhost");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                app.UseHangfireDashboard(options: new DashboardOptions
                {
                    Authorization = new[] { new HangfireDashboardAuthFilter() }
                });
            });
        }
    }
}
