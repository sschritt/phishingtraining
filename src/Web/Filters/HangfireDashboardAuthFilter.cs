using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using PhishingTraining.Web.Helpers.Security;

namespace PhishingTraining.Web.Filters
{
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var authResult = httpContext.AuthenticateAsync().GetAwaiter().GetResult();

            return authResult.Succeeded && (authResult.Principal?.IsInRole(Roles.Admin) ?? false);
        }
    }
}