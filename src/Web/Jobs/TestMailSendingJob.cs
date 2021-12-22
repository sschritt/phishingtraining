using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Services;

namespace PhishingTraining.Web.Jobs
{
    public class TestMailSendingJob : BackgroundJobWithActivationState<TestMailSendingJob>
    {
        private IConfiguration Configuration { get; }
        private IEmailSender EmailSender { get; }

        protected override HostedServiceActivationState ActivationState => Configuration.TestMailSendingJob();

        public TestMailSendingJob(ILogger<TestMailSendingJob> logger, IConfiguration configuration, IEmailSender emailSender) :
            base(logger)
        {
            Configuration = configuration;
            EmailSender = emailSender;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await EmailSender.SendEmailAsync("phishing-training@example.com", "Testmail", @"Hello this is a test message sent by MailKit via my awesome docker compose config");
        }
    }
}