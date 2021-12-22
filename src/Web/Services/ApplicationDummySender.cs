using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Queries;
using PhishingTraining.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Services
{
    public class ApplicationDummySender : IApplicationDummySender
    {
        public ApplicationDummySender(ILogger<ApplicationDummySender> logger, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            Logger = logger;
            Configuration = configuration;
        }

        private ApplicationDbContext DbContext { get; }
        protected ILogger Logger { get; }
        protected IConfiguration Configuration { get; }

        public void SendPhishingMessage(Guid phishingMessageId)
        {
            BackgroundJob.Enqueue(() => SendDummyInternalAsync(phishingMessageId, false, CancellationToken.None));
        }

        public void SendTestPhishingMessage(Guid phishingMessageId, string user)
        {
            Logger.LogInformation($"Dummy test clicked for phishingMessageId {phishingMessageId} by {user}");
        }

        public async Task SendDummyInternalAsync(Guid phishingMessageId, bool test, CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Dummy message send for phishingMessageId {phishingMessageId}");

            var phishingMessage = await DbContext.Query<PhishingMessage>().WhereId(phishingMessageId).IncludeUser().IncludeTemplate().SingleAsync(cancellationToken);
            
            try
            {
                if (!test)
                {
                    phishingMessage.TimeSent = DateTimeOffset.Now;
                    await DbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Couldn't send dummy message");
                throw;
            }

            Logger.LogDebug("Sent dummy message (Test: {test})", test);
        }
    }
}