using CM.Text;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Queries;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Services.Interfaces;

namespace PhishingTraining.Web.Services
{
    public class ApplicationSmsSender : ServiceBase, IApplicationSmsSender
    {
        private ILogger<ApplicationSmsSender> Logger { get; }
        public ApplicationSmsSender(IServiceProvider serviceProvider, ILogger<ApplicationSmsSender> logger) : base(serviceProvider)
        {
            Logger = logger;
        }

        public void SendPhishingMessage(Guid phishingMessageId)
        {
            BackgroundJob.Enqueue(() => SendPhishingSmsInternalAsync(phishingMessageId, null, false, CancellationToken.None));
        }

        public void SendTestPhishingMessage(Guid phishingMessageId, string phoneNumber)
        {
            BackgroundJob.Enqueue(() => SendPhishingSmsInternalAsync(phishingMessageId, phoneNumber, true, CancellationToken.None));
        }

        public async Task SendPhishingSmsInternalAsync(Guid phishingMessageId, string phoneNumber, bool test, CancellationToken cancellationToken)
        {
            var phishingMessage = await DbContext.Query<PhishingMessage>().WhereId(phishingMessageId).IncludeUser().IncludeTemplate().SingleAsync(cancellationToken);

            var receiverPhoneNumber = phoneNumber ?? phishingMessage.User.PhoneNumber;

            if (String.IsNullOrEmpty(receiverPhoneNumber))
            {
                Logger.LogWarning($"Could not send message {phishingMessageId} with template {phishingMessage.PhishingTemplate.Name}, phone number missing");
                return;
            }

            if ( String.IsNullOrEmpty(phishingMessage.TextBody) || String.IsNullOrEmpty(phishingMessage.PhishingTemplate.SenderName))
            {
                Logger.LogError($"Could not send message {phishingMessageId} with template {phishingMessage.PhishingTemplate.Name}");
                return;
            }

            // Sender alphanumeric ID is max. 11 characters in Austria
            var senderName = phishingMessage.SenderName ?? String.Empty;
            var sender = senderName.Length > 11 ? senderName.Substring(0, 11) : senderName;
            var body = phishingMessage.TextBody;

            Logger.LogInformation($"SMS prepared for sending {phishingMessage.PhishingTemplate.Name} to {receiverPhoneNumber} by sender: {sender} and body: {body}");

            try
            {
                var client = new TextClient(new Guid(Configuration.SmsProviderApiKey()));
                var result = await client.SendMessageAsync(body, sender, new List<string> { receiverPhoneNumber }, phishingMessage.PhishingTemplate.Name).ConfigureAwait(false);
                Logger.LogInformation($"SMS sending result: {result.statusMessage} and code: {result.statusCode}");

                if (!test)
                {
                    phishingMessage.TimeSent = DateTimeOffset.Now;
                    await DbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Couldn't send sms");
                throw;
            }
            Logger.LogInformation($"Sent sms template {phishingMessage.PhishingTemplate.Name} to {receiverPhoneNumber}");
        }
    }
}
