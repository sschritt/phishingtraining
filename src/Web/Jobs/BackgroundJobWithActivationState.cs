using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Helpers;

namespace PhishingTraining.Web.Jobs
{
    public abstract class BackgroundJobWithActivationState<TJob>
    {
        protected abstract HostedServiceActivationState ActivationState { get; }
        protected ILogger<TJob> Logger { get; }

        protected BackgroundJobWithActivationState(ILogger<TJob> logger)
        {
            Logger = logger;
        }

        [DisableConcurrentExecution(600)]
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (ActivationState == HostedServiceActivationState.enabled)
            {
                await RunAsync(cancellationToken);
            }
            else
            {
                Logger.LogDebug("Skipping {HostedService} as it's disabled.", GetType().FullName);
            }
        }

        protected abstract Task RunAsync(CancellationToken cancellationToken);
    }
}