using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Enums;

namespace PhishingTraining.Web.Queries
{
    public static class PhishingMessageQueries
    {
        public static IQueryable<PhishingMessage> WhereCampaignId(this IQueryable<PhishingMessage> query,
            Guid campaignId) => query.Where(message => message.CampaignId == campaignId);

        public static IQueryable<PhishingMessage> WhereId(this IQueryable<PhishingMessage> query,
            Guid id) => query.Where(message => message.Id == id);

        public static IIncludableQueryable<PhishingMessage, ApplicationUser> IncludeUser(this IQueryable<PhishingMessage> query) =>
            query.Include(message => message.User);

        public static IIncludableQueryable<PhishingMessage, PhishingTemplate> IncludeTemplate(this IQueryable<PhishingMessage> query) =>
            query.Include(message => message.PhishingTemplate);

        public static IQueryable<PhishingMessage> WhereUnsentEmailsAreDueToSend(this IQueryable<PhishingMessage> query) => query.Where(message => message.PhishingTemplate.Type == TemplateType.Email && message.TimeSent == null && message.TimeToSend <= DateTimeOffset.Now);

        public static IQueryable<PhishingMessage> WhereUnsentMessagesAreDueToSend(this IQueryable<PhishingMessage> query) => query.Where(message => message.TimeSent == null && message.TimeToSend <= DateTimeOffset.Now);


        public static IQueryable<PhishingMessage> WhereUnsent(this IQueryable<PhishingMessage> query) => query.Where(message => message.TimeSent == null || message.TimeSent == DateTimeOffset.MinValue);
        
        public static IQueryable<PhishingMessage> WhereSent(this IQueryable<PhishingMessage> query) => query.Where(message => message.TimeSent != null && message.TimeSent != DateTimeOffset.MinValue);
        public static IQueryable<PhishingMessage> WhereTemplateId(this IQueryable<PhishingMessage> query, Guid templateId) => query.Where(message => message.PhishingTemplateId == templateId);

        public static IQueryable<PhishingMessage> IncludeCampaign(this IQueryable<PhishingMessage> query) =>
            query.Include(x => x.Campaign);
    }
}