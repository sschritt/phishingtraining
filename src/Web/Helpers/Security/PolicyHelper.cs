using Microsoft.Extensions.DependencyInjection;

namespace PhishingTraining.Web.Helpers.Security
{
    public static class PolicyHelper
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.TemplateManagement,
                    builder => builder.RequireClaim(FeatureClaims.TemplateManagement));
                options.AddPolicy(Policies.CampaignManagement,
                    builder => builder.RequireClaim(FeatureClaims.CampaignManagement));
                options.AddPolicy(Policies.IsParticipant,builder => builder.RequireRole(Roles.Participant));
                options.AddPolicy(Policies.IsAdministrator, builder => builder.RequireRole(Roles.Admin));
            });
            return services;
        }
    }
}