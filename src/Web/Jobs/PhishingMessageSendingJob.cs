using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Queries;
using PhishingTraining.Web.Services;
using PhishingTraining.Web.Services.Interfaces;

namespace PhishingTraining.Web.Jobs
{
    public class PhishingMessageSendingJob
    {
        private ApplicationDbContext DbContext { get; }
        private ILogger<PhishingMessageSendingJob> Logger { get; }
        private IApplicationEmailSender MailSender { get; }
        private IApplicationSmsSender SmsSender { get; }
        private IApplicationDummySender DummySender { get; }

        public PhishingMessageSendingJob(ApplicationDbContext dbContext, ILogger<PhishingMessageSendingJob> logger, IApplicationEmailSender mailSender, IApplicationSmsSender smsSender, IApplicationDummySender dummySender)
        {
            DbContext = dbContext;
            Logger = logger;
            MailSender = mailSender;
            SmsSender = smsSender;
            DummySender = dummySender;
        }

        [DisableConcurrentExecution(3600)]
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var messagesToSend = await DbContext.Query<PhishingMessage>().WhereUnsentMessagesAreDueToSend().IncludeTemplate().ToListAsync(cancellationToken);

            foreach (var msg in messagesToSend)
            {
                switch (msg.PhishingTemplate.Type)
                {
                    case Enums.TemplateType.Email: 
                        MailSender.SendPhishingMessage(msg.Id); 
                        break;
                    case Enums.TemplateType.Sms:
                        SmsSender.SendPhishingMessage(msg.Id);
                        break;
                    default: 
                        Logger.LogInformation($"No sender available for: {msg.PhishingTemplate.Name}, use dummy sender");
                        DummySender.SendPhishingMessage(msg.Id);
                        break;
                };
            }

            Logger.LogDebug("Enqued {Count} phishing emails for sending.", messagesToSend.Count);
        }
    }
}
