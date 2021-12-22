using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Helpers;

namespace PhishingTraining.Web.Jobs
{
    public class DatabaseMigrationJob : BackgroundJobWithActivationState<DatabaseMigrationJob>
    {
        private IConfiguration Configuration { get; }
        private ApplicationDbContext DbContext { get; }

        public DatabaseMigrationJob(ILogger<DatabaseMigrationJob> logger, IConfiguration configuration, ApplicationDbContext dbContext) : base(logger)
        {
            Configuration = configuration;
            DbContext = dbContext;
        }

        protected override HostedServiceActivationState ActivationState => Configuration.DatabaseMigrationJob();
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Started DB migration..");

            while (!await DbContext.Database.CanConnectAsync(cancellationToken))
            {
                //when running with docker compose the DB is taking longer than the webapp to startup. Waiting until the DB is ready
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            await DbContext.Database.MigrateAsync(cancellationToken);

            Logger.LogInformation("Finished DB migration!");
        }
    }
}