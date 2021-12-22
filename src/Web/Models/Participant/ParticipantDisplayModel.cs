using PhishingTraining.Web.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Participant
{
    public class ParticipantDisplayModel
    {
        public Guid Id { get; set; }
        [Display(Name = "User")]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? Birthdate { get; set; }
        public GenderType Gender { get; set; }
        public string MotherFirstName { get; set; }
        public string FatherFirstName { get; set; }
        [Display(Name = "Pet")]
        public string PetName { get; set; }

        public string Email { get; set; }
        [Display(Name = "Phone")]
        public  string PhoneNumber { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [Display(Name = "Instagram")]
        public string InstagramUser { get; set; }
        [Display(Name = "Facebook")]
        public string FacebookUser { get; set; }
        [Display(Name = "TikTok")]
        public string TikTokUser { get; set; }
        [Display(Name = "Real")]
        public bool RealData { get; set; }

        public Guid CampaignId { get; set; }

        public string this[string propName] => GetType().GetProperty(propName)?.GetValue(this, null)?.ToString() ?? string.Empty;
    }
}
