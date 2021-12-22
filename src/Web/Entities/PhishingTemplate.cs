using PhishingTraining.Web.Entities.Base;
using PhishingTraining.Web.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhishingTraining.Web.Entities
{
    public class PhishingTemplate : NamedEntity
    {
        public TemplateType Type { get; set; }

        [MaxLength(200)]
        public string SenderName { get; set; }
        [MaxLength(200)]
        public string SenderAddress { get; set; }
        public bool SenderKnown { get; set; }
        public IncentiveType Incentive { get; set; } 
        public string Origin { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public string HtmlTemplateFilename { get; set; }
        public string PlainTemplateFilename { get; set; }
        [MaxLength(200)]
        public string SubjectTemplate { get; set; }
        public int MinSecondsBetweenMessages { get; set; }
        public TimeSpan? FromTimeOfDay { get; set; }
        public TimeSpan? ToTimeOfDay { get; set; }
        public TemplateSendType SendType { get; set; }
        public DifficultyType Difficulty { get; set; }
        public IList<UserMetadata> Parameters { get; set; } = new List<UserMetadata>();
        public IList<Campaign> CampaignUsage { get; set; } = new List<Campaign>();
        public string EducationalInformation { get; set; }
    }
}