using System;
using PhishingTraining.Web.Entities.Base;
using PhishingTraining.Web.Enums;

namespace PhishingTraining.Web.Entities
{
    public class EventLog : KeyedEntity
    {
        public Guid? UserId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? PhishingTemplateId { get; set; }
        public Guid? PhishingMessageId { get; set; }

        public EventType Type { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}