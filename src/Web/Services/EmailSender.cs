using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using MimeKit;
using PhishingTraining.Web.Helpers;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace PhishingTraining.Web.Services
{
    public class EmailSender : EmailSenderBase, IEmailSender
    {
        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration) : base(logger, configuration) { }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            BackgroundJob.Enqueue(() => SendEmailInternalAsync(email, subject, htmlMessage, CancellationToken.None));
            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is only supposed to be called from Hangfire context as it doesn't catch any exceptions.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="htmlMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendEmailInternalAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken)
        {
            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            var message = new MimeMessage
            {
                Subject = subject,
                Body = builder.ToMessageBody()
            };
            message.From.Add(MailboxAddress.Parse(Configuration.IdentitySenderAddress()));
            message.To.Add(MailboxAddress.Parse(email));

            await SendMimeMessageAsync(message, cancellationToken);
        }
    }
}
