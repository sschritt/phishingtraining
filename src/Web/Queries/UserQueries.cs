using System;
using System.Linq;
using PhishingTraining.Web.Entities;

namespace PhishingTraining.Web.Queries
{
    public static class UserQueries
    {
        public static IQueryable<Campaign> MyManagedCampaigns(this IQueryable<ApplicationUser> query, Guid userId) =>
            query.Where(user => user.Id == userId)
                .SelectMany(user => user.ManagedCampaigns);

        public static IQueryable<Campaign> MyParticipatingCampaigns(this IQueryable<ApplicationUser> query, Guid userId) =>
            query.Where(user => user.Id == userId)
                .SelectMany(user => user.ParticipatingCampaigns);
    }
}