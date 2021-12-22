using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.PhishingTemplate
{
    public class PhishingTemplateDisplayModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Origin { get; set; }

        public string Description { get; set; }
        public TemplateType Type { get; set; }
        [UIHint("DateTimeOffsetDayMonth")]
        public DateTimeOffset? From { get; set; }
        [UIHint("DateTimeOffsetDayMonth")]
        public DateTimeOffset? To { get; set; }
        public int MinSecondsBetweenMessages { get; set; }
        public TimeSpan? FromTimeOfDay { get; set; }
        public TimeSpan? ToTimeOfDay { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public bool SenderKnown { get; set; }
        public IncentiveType Incentive { get; set; }
        public TemplateSendType SendType { get; set; }
        public DifficultyType Difficulty { get; set; }
        public string SubjectTemplate { get; set; }

        [Display(Name = nameof(IsChecked))]
        public bool IsChecked { get; set; }
        
        [Display(Name = "HTML")]
        public bool HasTemplateFile { get; set; }

        [Display(Name = "Text")]
        public bool HasPlainTemplateFile { get; set; }
    }
}
