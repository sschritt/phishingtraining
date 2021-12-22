using System;
using System.ComponentModel.DataAnnotations;
using PhishingTraining.Web.Entities.Base;

namespace PhishingTraining.Web.Entities
{
    public class PhishingMessage : KeyedEntity
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; }

        public Guid? PhishingTemplateId { get; set; }
        public PhishingTemplate PhishingTemplate { get; set; }
        public DateTimeOffset TimeToSend { get; set; }
        public DateTimeOffset? TimeSent { get; set; }

        public DateTimeOffset? ClickDate { get; set; }
        public DateTimeOffset? FetchImageDate { get; set; }

        [MaxLength(int.MaxValue)]
        public string HtmlBody { get; set; }
        [MaxLength(int.MaxValue)]
        public string TextBody { get; set; }
        [MaxLength(200)]
        public string Subject { get; set; }
        [MaxLength(200)]
        public string SenderName { get; set; }

        public string ClickLocation { get; set; }
        public string ClickActivity { get; set; }
        public string ClickCompany { get; set; }

        public bool SurveyIsEmpty
            => string.IsNullOrWhiteSpace(ClickLocation) && string.IsNullOrWhiteSpace(ClickActivity) && string.IsNullOrWhiteSpace(ClickCompany);
    }
}