using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PhishingTraining.Web.Models.Participant;
using PhishingTraining.Web.Models.PhishingTemplate;

namespace PhishingTraining.Web.Models.Campaign
{
    public class CampaignEditModel
    {
        public Guid? Id { get; set; }

        [Display(Name = nameof(Name))]
        public string Name { get; set; }

        [Display(Name = nameof(Description))]
        public string Description { get; set; }

        [Display(Name = nameof(Start))]
        public DateTimeOffset Start { get; set; }

        [Display(Name = nameof(End))]
        public DateTimeOffset End { get; set; }

        [MaxLength(200)]
        public string School { get; set; }

        [MaxLength(200)]
        public string Director { get; set; }

        [MaxLength(200)]
        public string Class { get; set; }

        [MaxLength(200)]
        public string ClassTeacher { get; set; }

        [MaxLength(200)]
        public string InformaticsTeacher { get; set; }

        [Display(Name = nameof(NumberOfMessagesPerParticipant))]
        public int NumberOfMessagesPerParticipant { get; set; }

        [Display(Name = nameof(NumberOfEMailMessagesPerParticipant))]
        public int NumberOfEMailMessagesPerParticipant { get; set; }

        [Display(Name = nameof(NumberOfSMSMessagesPerParticipant))]
        public int NumberOfSMSMessagesPerParticipant { get; set; }

        [Display(Name = nameof(NumberOfInstagramMessagesPerParticipant))]
        public int NumberOfInstagramMessagesPerParticipant { get; set; }

        [Display(Name = nameof(NumberOfTikTokMessagesPerParticipant))]
        public int NumberOfTikTokMessagesPerParticipant { get; set; }

        [Display(Name = nameof(NumberOfSnapchatMessagesPerParticipant))]
        public int NumberOfSnapchatMessagesPerParticipant { get; set; }

        [Display(Name = nameof(NumberOfWhatsAppMessagesPerParticipant))]
        public int NumberOfWhatsAppMessagesPerParticipant { get; set; }
        [Display(Name = nameof(NumberOfFacebookMessagesPerParticipant))]
        public int NumberOfFacebookMessagesPerParticipant { get; set; }

        [Display(Name = nameof(ShowHintsOnClick))]
        public bool ShowHintsOnClick { get; set; }
    }
}