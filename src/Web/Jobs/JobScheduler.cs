using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace PhishingTraining.Web.Jobs
{
    public class JobScheduler : BackgroundService
    {
        /// <summary>
        /// By injecting <see cref="JobStorage"/> we make sure hangfire is fully initialized when running TaskScheduler.
        /// </summary>
        private JobStorage JobStorage { get; }

        public JobScheduler(JobStorage jobStorage)
        {
            JobStorage = jobStorage;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            JobStorage.Current = JobStorage;
            var jobId = BackgroundJob.Schedule<DatabaseMigrationJob>(service => service.StartAsync(CancellationToken.None), TimeSpan.Zero);
            jobId = BackgroundJob.ContinueJobWith<DatabaseSeedingJob>(jobId, service => service.StartAsync(CancellationToken.None));
            jobId = BackgroundJob.ContinueJobWith<TestMailSendingJob>(jobId, service => service.StartAsync(CancellationToken.None));

            //Run every 15 min
            RecurringJob.AddOrUpdate<PhishingMessageSendingJob>(job => job.StartAsync(CancellationToken.None), "*/15 * * * *");

            return Task.CompletedTask;
        }
    }
}
