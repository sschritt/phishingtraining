using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Models.PhishingTemplate;
using System;
using System.ComponentModel.DataAnnotations;
using PhishingTraining.Web.Models.Campaign;

namespace PhishingTraining.Web.Models.PhishingMessage
{
    public class PhishingMessageModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public CampaignModel Campaign { get; set; }
        public PhishingTemplateModel PhishingTemplate { get; set; }

        public ApplicationUser User { get; set; }
        public string Subject { get; set; }
        public string SenderName { get; set; }

        public Guid PhishingTemplateId { get; set; }

        [UIHint("DateTimeOffsetIncludingTime")]
        public DateTimeOffset TimeToSend { get; set; }
        [UIHint("DateTimeOffsetIncludingTime")]
        public DateTimeOffset? TimeSent { get; set; }
        [UIHint("DateTimeOffsetIncludingTime")]
        public DateTimeOffset? ClickDate { get; set; }
        [UIHint("DateTimeOffsetIncludingTime")]
        public DateTimeOffset? FetchImageDate { get; set; }
        //public DateTimeOffset? ClickDateClean() => (this.ClickDate != null && this.ClickDate > DateTimeOffset.MinValue) ? this.ClickDate : null;
        public string ClickLocation { get; set; }
        public string ClickSituation { get; set; }
        public bool SurveyIsEmpty
            => string.IsNullOrWhiteSpace(ClickLocation) && string.IsNullOrWhiteSpace(ClickSituation);

        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
    }
}
