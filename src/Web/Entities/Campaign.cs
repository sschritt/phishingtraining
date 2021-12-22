using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PhishingTraining.Web.Entities.Base;

namespace PhishingTraining.Web.Entities
{
    public class Campaign : NamedEntity
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public int NumberOfEMailMessagesPerParticipant { get; set; }
        public int NumberOfSMSMessagesPerParticipant { get; set; }
        public int NumberOfInstagramMessagesPerParticipant { get; set; }
        public int NumberOfTikTokMessagesPerParticipant { get; set; }
        public int NumberOfSnapchatMessagesPerParticipant { get; set; }
        public int NumberOfFacebookMessagesPerParticipant { get; set; }
        public int NumberOfWhatsAppMessagesPerParticipant { get; set; }
        public string ClassTeacher { get; set; }
        public string School { get; set; }
        public string Class { get; set; }
        public string InformaticsTeacher { get; set; }
        public string Director { get; set; }
        public bool ShowHintsOnClick { get; set; }

        public IList<ApplicationUser> Managers { get; set; } = new List<ApplicationUser>();
        public IList<ApplicationUser> Participants { get; set; } = new List<ApplicationUser>();
        public IList<PhishingTemplate> TemplateUsage { get; set; } = new List<PhishingTemplate>();
    }
}
