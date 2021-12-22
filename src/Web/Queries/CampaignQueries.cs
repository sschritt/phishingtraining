using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhishingTraining.Web.Entities;

namespace PhishingTraining.Web.Queries
{
    public static class CampaignQueries
    {
        public static IIncludableQueryable<Campaign, IList<ApplicationUser>> IncludeParticipants(this IQueryable<Campaign> query) =>
            query.Include(campaign => campaign.Participants);
        public static IIncludableQueryable<Campaign, IList<ApplicationUser>> IncludeManagers(this IQueryable<Campaign> query) =>
            query.Include(campaign => campaign.Managers);
        public static IIncludableQueryable<Campaign, IList<PhishingTemplate>> IncludeTemplates(this IQueryable<Campaign> query) =>
            query.Include(campaign => campaign.TemplateUsage);
    }
}
