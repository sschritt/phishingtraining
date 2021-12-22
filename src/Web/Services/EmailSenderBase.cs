using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PhishingTraining.Web.Helpers;

namespace PhishingTraining.Web.Services
{
    public class EmailSenderBase
    {
        public EmailSenderBase(ILogger logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }

        protected ILogger Logger { get; }
        protected IConfiguration Configuration { get; }

        protected async Task SendMimeMessageAsync(MimeMessage message, CancellationToken cancellationToken, string sender)
        {
            if (!String.IsNullOrEmpty(Configuration.MailServerFakeHost()))
            {
                await SendMimeMessageAsync(message, cancellationToken, Configuration.MailServerFakeHost(),
                    Configuration.MailServerFakePort(), Configuration.MailServerFakeUseSsl(), Configuration.MailServerFakeNeedsAuthentication(),
                    sender, Configuration.MailServerFakePassword());
            }
            else
            {
                await SendMimeMessageAsync(message, cancellationToken);
            }
        }

        protected async Task SendMimeMessageAsync(MimeMessage message, CancellationToken cancellationToken)
        {
            await SendMimeMessageAsync(message, cancellationToken, Configuration.MailServerHost(),
                Configuration.MailServerPort(), Configuration.MailServerUseSsl(), Configuration.MailServerNeedsAuthentication(),
                Configuration.MailServerUser(), Configuration.MailServerPassword());
        }

        private async Task SendMimeMessageAsync(MimeMessage message, CancellationToken cancellationToken, string host, int port, bool useSSL, bool needsAuthenticationToken, string user, string password)
        {
            try
            {
                using var client = new SmtpClient();
                Logger.LogDebug("Connect mail client {host} {port} {user}.");
                await client.ConnectAsync(host, port, useSSL, cancellationToken);
                if (needsAuthenticationToken)
                {
                    Logger.LogDebug("Authenticate mail client server {user}.");
                    await client.AuthenticateAsync(user, password,
                        cancellationToken);
                }

                Logger.LogDebug($"Send message {message.Subject}.");

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}