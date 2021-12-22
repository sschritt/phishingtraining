using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Helpers.Security;
using PhishingTraining.Web.Queries;

namespace PhishingTraining.Web.Jobs
{
    public class DatabaseSeedingJob : BackgroundJobWithActivationState<DatabaseSeedingJob>
    {
        public const string UserNameAdmin = "admin";
        public const string UserNameManager = "manager";
        public const string UserNameParticipant = "participant";

        private IConfiguration Configuration { get; }
        private ApplicationDbContext DbContext { get; }
        private UserManager<ApplicationUser> UserManager { get; }
        private RoleManager<ApplicationRole> RoleManager { get; }
        private IWebHostEnvironment HostEnvironment { get; }

        protected override HostedServiceActivationState ActivationState => Configuration.DatabaseSeedingJob();

        public DatabaseSeedingJob(ILogger<DatabaseSeedingJob> logger, ApplicationDbContext dbContext,
            IConfiguration configuration, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment) : base(logger)
        {
            DbContext = dbContext;
            Configuration = configuration;
            RoleManager = roleManager;
            UserManager = userManager;
            HostEnvironment = hostEnvironment;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Started seeding data..");

            await EnsureRolesAndUsers(cancellationToken);
            var userMetadata = await EnsureMetadata(cancellationToken);
            if (HostEnvironment.IsDevelopment())
            {
                await EnsureTestCampaign(UserNameManager, UserNameParticipant, UserNameAdmin, userMetadata);
            }

            Logger.LogInformation("Finished seeding data!");
        }

        private async Task EnsureRolesAndUsers(CancellationToken cancellationToken)
        {
            await EnsureRole(Roles.Participant, FeatureClaims.ParticipantFeatureClaims);
            await EnsureRole(Roles.Manager, FeatureClaims.ManagerFeatureClaims);
            await EnsureRole(Roles.Admin, FeatureClaims.AdminFeatureClaims);

            await EnsureUser(UserNameAdmin, "admin@example.com", Roles.Admin, "VwE3AdadTta6L1Jodgfj");
            await EnsureUser(UserNameManager, "manager@example.com", Roles.Manager, "3Rnk0hq5kAqoZ19eQdI6");
            await EnsureUser(UserNameParticipant, "participant@example.com", Roles.Participant, "lGAVsNavtmIDOKyME0hP");

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<UserMetadata[]> EnsureMetadata(CancellationToken cancellationToken)
        {
            var m1 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("B9C6F050-C79C-4AB0-B864-DED4EBA3F814"),
                TemplatePlaceholder = nameof(ApplicationUser.FirstName)
            });
            var m2 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("ABFC5152-32C3-4A2F-AFC7-12E22DF6F505"),
                TemplatePlaceholder = nameof(ApplicationUser.LastName)
            });
            var m3 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("63C85866-D991-4B55-AA3A-41116E685C35"),
                TemplatePlaceholder = nameof(ApplicationUser.FatherFirstName)
            });
            var m4 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("B15A23AD-E332-426A-AE49-66EBD71080C1"),
                TemplatePlaceholder = nameof(ApplicationUser.MotherFirstName)
            });
            var m5 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("C84BACFB-E88B-42A6-8441-90DAB4650D2C"),
                TemplatePlaceholder = nameof(ApplicationUser.PetName)
            });
            var m6 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("9434D2BE-1BA6-42BD-8020-2CC1F940B0FA"),
                TemplatePlaceholder = nameof(ApplicationUser.Street)
            });
            var m7 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("D6F26EF8-BC73-4659-AEE2-AE20498C2E62"),
                TemplatePlaceholder = nameof(ApplicationUser.City)
            });
            var m8 = await DbContext.EnsureKeyedEntityExists(new UserMetadata()
            {
                Id = Guid.Parse("D829D23A-CC25-463D-A829-BE841CDD13AF"),
                TemplatePlaceholder = nameof(ApplicationUser.Country)
            });
            await DbContext.SaveChangesAsync(cancellationToken);
            return new[] { m1, m2, m3, m4, m5, m6, m7, m8 };
        }

        private async Task EnsureTestCampaign(string userName1, string userName2, string userName3, UserMetadata[] userMetadata)
        {
            var user1 = await UserManager.FindByNameAsync(userName1);
            var user2 = await UserManager.FindByNameAsync(userName2);
            var user3 = await UserManager.FindByNameAsync(userName3);

            if (user1 == null)
            {
                throw new ArgumentException($"User missing {userName1}", nameof(userName1));
            }

            if (user2 == null)
            {
                throw new ArgumentException($"User missing {userName2}", nameof(userName2));
            }

            if (user3 == null)
            {
                throw new ArgumentException($"User missing {userName3}", nameof(userName3));
            }

            var template1 = await DbContext.EnsureKeyedEntityExists(new PhishingTemplate
            {
                Id = Guid.Parse("d387ec94-86a9-409f-98e0-bf4532c9866b"),
                Name = "Lotto win",
                SenderAddress = "lotto@test.com",
                Description = "Pretend you have a jackpot.",
                SenderName = "Lotto AG",
                SubjectTemplate = "Congratulation to your Lotto win {FirstName} {LastName}",
                Difficulty = Enums.DifficultyType.Easy,
                SendType = Enums.TemplateSendType.Individual,
                Type = Enums.TemplateType.Email,
                From = new DateTime(2021, 01, 01),
                To = new DateTime(2021, 12, 31),
                FromTimeOfDay = new TimeSpan(8, 0, 0),
                ToTimeOfDay = new TimeSpan(18, 0, 0),
                Origin = "https://127.0.0.1:41443",
                MinSecondsBetweenMessages = 0
            });
            var template2 = await DbContext.EnsureKeyedEntityExists(new PhishingTemplate
            {
                Id = Guid.Parse("149e14d0-3114-4e6d-9318-cd44fcddd7ab"),
                Name = "Whatsapp scandal at school during summer holidays",
                SenderAddress = "xy@gymnasium.at",
                Description = "Claim a scandal happened in the summer holidays.",
                SenderName = "SchuleXY",
                SubjectTemplate = "Wow, imagine what we found out about your father {FatherFirstName}!",
                Type = Enums.TemplateType.Email,
                Difficulty = Enums.DifficultyType.Hard,
                SendType = Enums.TemplateSendType.Group,
                From = new DateTime(2021, 01, 01),
                To = new DateTime(2021, 03, 01),
                FromTimeOfDay = new TimeSpan(8, 0, 0),
                ToTimeOfDay = new TimeSpan(18, 0, 0),
                Origin = "https://127.0.0.1:41443",
                MinSecondsBetweenMessages = 0
            });
            var template3 = await DbContext.EnsureKeyedEntityExists(new PhishingTemplate
            {
                Id = Guid.Parse("020f0152-c725-486d-be53-570fefc4def7"),
                Name = "New friend request.",
                SenderAddress = "schoolfriend@gmail.com",
                Description = "New friend request from a school friend.",
                SenderName = "TU",
                SubjectTemplate = "Your schoolmate {PetName} sent a friend request! Visit {Domain}/Feedback/Click/{MessageId}",
                Type = Enums.TemplateType.Sms,
                Difficulty = Enums.DifficultyType.VeryEasy,
                SendType = Enums.TemplateSendType.Individual,
                From = new DateTime(2021, 01, 01),
                To = new DateTime(2021, 04, 30),
                FromTimeOfDay = new TimeSpan(8, 0, 0),
                ToTimeOfDay = new TimeSpan(18, 0, 0),
                Origin = "https://127.0.0.1:41443",
                MinSecondsBetweenMessages = 0
            });
            var template4 = await DbContext.EnsureKeyedEntityExists(new PhishingTemplate
            {
                Id = Guid.Parse("10235F7B-D965-44FB-AFA4-12BB0189B92B"),
                Name = "Christmas present PS5",
                SenderAddress = "parents@something.at",
                Description = "Christmas present from your parents: new PS5",
                SenderName = "Parents",
                SubjectTemplate = "Mom {MotherFirstName} and your father {FatherFirstName} have a present for you!",
                Type = Enums.TemplateType.Email,
                Difficulty = Enums.DifficultyType.VeryHard,
                SendType = Enums.TemplateSendType.Individual,
                From = new DateTime(2021, 06, 01),
                To = new DateTime(2021, 10, 01),
                FromTimeOfDay = new TimeSpan(8, 0, 0),
                ToTimeOfDay = new TimeSpan(18, 0, 0),
                Origin = "https://127.0.0.1:41443",
                MinSecondsBetweenMessages = 0
            });

            await DbContext.SaveChangesAsync();
            await EnsureTemplateParametersAsync(template1.Id, userMetadata.Take(4).ToArray());
            await EnsureTemplateParametersAsync(template2.Id, userMetadata);
            await EnsureTemplateParametersAsync(template3.Id, userMetadata);
            await EnsureTemplateParametersAsync(template4.Id, userMetadata);

            var campaign = await DbContext.EnsureKeyedEntityExists(new Campaign
            {
                Id = Guid.Parse("4ae3e9c9-bcf6-4bab-ba4a-c40ab6c3f7fe"),
                Name = "Test campaign",
                Description = "Test",
                Start = new DateTime(2021, 01, 01),
                End = new DateTime(2021, 01, 01).AddDays(+160),
                Managers = new List<ApplicationUser> { user1 },
                Participants = new List<ApplicationUser> { user2, user3 },
                TemplateUsage = new List<PhishingTemplate> { template1, template2 },
                NumberOfSMSMessagesPerParticipant = 2,
                NumberOfEMailMessagesPerParticipant = 2
            });
            var message1 = await DbContext.EnsureKeyedEntityExists(new PhishingMessage
            {
                Id = Guid.Parse("12b59df1-1ea7-4b45-8d1c-ea62871d7c4a"),
                CampaignId = campaign.Id,
                PhishingTemplateId = template1.Id,
                UserId = user2.Id,
                TimeToSend = new DateTime(2021, 10, 29).AddDays(-5),
                TimeSent = new DateTime(2021, 10, 29),
                HtmlBody = "<strong>from your parents: Han &amp; Leia</strong>",
                Subject = "Congratulation to your Lotto win Ben Solo"
            });
            //var message2 = await DbContext.EnsureKeyedEntityExists(new PhishingMessage
            //{
            //    Id = Guid.Parse("06a7c4dd-740a-47fc-9979-3092dbcdb04b"),
            //    CampaignId = campaign.Id,
            //    PhishingTemplateId = template2.Id,
            //    UserId = user2.Id,
            //    TimeToSend = new DateTime(2021, 10, 29).AddDays(-5)
            //});
            //var message3 = await DbContext.EnsureKeyedEntityExists(new PhishingMessage
            //{
            //    Id = Guid.Parse("f9b801e1-fb7f-4eab-b8f7-679e209d5b19"),
            //    CampaignId = campaign.Id,
            //    PhishingTemplateId = template1.Id,
            //    UserId = user2.Id,
            //    TimeToSend = new DateTime(2021, 10, 29).AddDays(-5),
            //    HtmlBody = "<strong>from your parents: Luke I am your father &amp; Padme</strong>",
            //    Subject = "Congratulation to your Lotto win Luke Skywalker"
            //});
            var message4 = await DbContext.EnsureKeyedEntityExists(new PhishingMessage
            {
                Id = Guid.Parse("240B5670-9CB6-47C4-85DA-9741AE0FB48D"),
                CampaignId = campaign.Id,
                PhishingTemplateId = template4.Id,
                UserId = user2.Id,
                TimeToSend = new DateTime(2021, 10, 29).AddDays(-3),
                TimeSent = new DateTime(2021, 10, 29).AddDays(-1),
                HtmlBody = "<strong>Mom Silvy and your father Joe have a present for you!",
                Subject = "Mom Silvy and your father Joe have a present for you!"
            });

            await DbContext.SaveChangesAsync();
        }

        private async Task EnsureTemplateParametersAsync(Guid templateId, params UserMetadata[] parameters)
        {
            var template = await DbContext.Query<PhishingTemplate>().Where(t => t.Id == templateId).IncludeParameters()
                .SingleAsync();
            foreach (var param in parameters)
            {
                if (template.Parameters.All(metadata => metadata.Id != param.Id))
                {
                    template.Parameters.Add(param);
                }
            }
        }

        private async Task EnsureUser(string userName, string email, string role, string password)
        {
            var user = await UserManager.FindByNameAsync(userName);
            IdentityResult result;
            if (user == null)
            {
                user = new ApplicationUser { UserName = userName, Email = email, EmailConfirmed = true };
                result = await UserManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new SeedingException(typeof(ApplicationUser), userName);
                }
            }

            if (!await UserManager.IsInRoleAsync(user, role))
            {
                result = await UserManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    throw new SeedingException($"Couldn't add role {role} to: {user?.UserName}");
                }
            }
            user.Birthdate = new DateTime(2021, 01, 01).AddYears(-16);
            user.FirstName = userName + "first";
            user.LastName = userName + "last";
            user.City = "Vienna";
            user.Country = "Austria";
            user.Street = "Some street 3";
            user.FatherFirstName = "Tony";
            user.MotherFirstName = "Clara";
            user.PetName = "Someone";
            user.PhoneNumber = "+43 3332 2565";
            await UserManager.UpdateAsync(user);
        }

        private async Task EnsureRole(string roleName, params string[] claimTypes)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new ApplicationRole { Name = roleName };
                var result = await RoleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new SeedingException(typeof(ApplicationRole), roleName);
                }
            }

            var claims = await RoleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                var result = await RoleManager.RemoveClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    throw new SeedingException(typeof(Claim), claim.Type);
                }
            }

            foreach (var claimType in claimTypes.Distinct())
            {
                var result = await RoleManager.AddClaimAsync(role, new Claim(claimType, string.Empty));
                if (!result.Succeeded)
                {
                    throw new SeedingException(typeof(Claim), claimType);
                }
            }
        }
    }

    public class SeedingException : Exception
    {
        public SeedingException(Type entity, object identifier) : base($"Couldn't seed {entity.Name}: {identifier}") { }
        public SeedingException(string message) : base(message) { }
    }
}