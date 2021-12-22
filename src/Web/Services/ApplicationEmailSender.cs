using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Queries;
using PhishingTraining.Web.Services.Interfaces;
using System.Linq;

namespace PhishingTraining.Web.Services
{
    public class ApplicationEmailSender : EmailSenderBase, IApplicationEmailSender
    {
        public ApplicationEmailSender(ILogger<ApplicationEmailSender> logger, IConfiguration configuration, ApplicationDbContext dbContext) : base(logger, configuration)
        {
            DbContext = dbContext;
        }

        private ApplicationDbContext DbContext { get; }

        public void SendPhishingMessage(Guid phishingMessageId)
        {
            BackgroundJob.Enqueue(() => SendPhishingEmailInternalAsync(phishingMessageId, null, false, CancellationToken.None));
        }

        public void SendTestPhishingMessage(Guid phishingMessageId, string email)
        {
            BackgroundJob.Enqueue(() => SendPhishingEmailInternalAsync(phishingMessageId, email, true, CancellationToken.None));
        }

        /// <summary>
        /// This method is only supposed to be called from Hangfire context as it doesn't catch any exceptions.
        /// </summary>
        /// <param name="phishingMessageId"></param>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendPhishingEmailInternalAsync(Guid phishingMessageId, string email, bool  test, CancellationToken cancellationToken)
        {
            var phishingMessage = await DbContext.Query<PhishingMessage>().WhereId(phishingMessageId).IncludeUser().IncludeTemplate().SingleAsync(cancellationToken);
            var builder = new BodyBuilder
            {
                HtmlBody = phishingMessage.HtmlBody,
                TextBody = phishingMessage.TextBody
            };
            var message = new MimeMessage
            {
                Subject = phishingMessage.Subject ?? string.Empty,
                Body = builder.ToMessageBody()
            };

            var receiverAddress = email ?? phishingMessage.User.Email;
            if (String.IsNullOrEmpty(receiverAddress))
            {
                Logger.LogWarning("Couldn't send mail, email address missing");
                return;
            }

            message.From.Add(new MailboxAddress(phishingMessage.SenderName ?? string.Empty, phishingMessage.PhishingTemplate.SenderAddress ?? string.Empty));
            message.To.Add(MailboxAddress.Parse(receiverAddress));

            try
            {
                if (!String.IsNullOrEmpty(phishingMessage.PhishingTemplate.SenderAddress))
                {
                    await SendMimeMessageAsync(message, cancellationToken, phishingMessage.PhishingTemplate.SenderAddress);
                }
                else
                {
                    await SendMimeMessageAsync(message, cancellationToken);
                }

                if (!test)
                {
                    phishingMessage.TimeSent = DateTimeOffset.Now;
                    await DbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Couldn't send mail");
                throw;
            }
            Logger.LogDebug("Sent mail about {subject} to {recipient} (Test: {test})", message.Subject, message.To.FirstOrDefault(), test);
        }
    }
}