using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using PhishingTraining.Web.Enums;

namespace PhishingTraining.Web.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [ProtectedPersonalData]
        public string FirstName { get; set; }
        [ProtectedPersonalData]
        public string LastName { get; set; }
        [ProtectedPersonalData]
        public string MotherFirstName { get; set; }     
        [ProtectedPersonalData]
        public string FatherFirstName { get; set; }
        [ProtectedPersonalData]
        public string PetName { get; set; }
        [ProtectedPersonalData]
        public DateTimeOffset? Birthdate { get; set; }
        [ProtectedPersonalData]
        public string Street { get; set; }
        [ProtectedPersonalData]
        public string City { get; set; }
        [ProtectedPersonalData]
        public string Country { get; set; }
        [ProtectedPersonalData]
        public GenderType Gender { get; set; }
        [ProtectedPersonalData]
        public string InstagramUser { get; set; }
        [ProtectedPersonalData]
        public string FacebookUser { get; set; }
        [ProtectedPersonalData]
        public string TikTokUser { get; set; }
        [ProtectedPersonalData]
        public bool RealData { get; set; }
        [ProtectedPersonalData]
        public PhoneOsType PhoneOs { get; set; }
        [ProtectedPersonalData] 
        public ComputerOsType ComputerOs { get; set; }
        [ProtectedPersonalData]
        public PhoneProviderType PhoneProvider { get; set; }

        public IList<Campaign> ManagedCampaigns { get; set; }
        public IList<Campaign> ParticipatingCampaigns { get; set; }
    }

    public static class ApplicationUserExtensions
    {
        public static string GetPropertyValue(this ApplicationUser applicationUser, string propertyName, IStringLocalizer localizer)
        {
            if (propertyName == "Birthdate")
            {
                return applicationUser.Birthdate.HasValue ? applicationUser.Birthdate.Value.Date.ToShortDateString() : string.Empty;
            }
            else
            {
                var type = typeof(ApplicationUser).GetProperty(propertyName);
                var value = type?.GetValue(applicationUser, null)?.ToString() ?? string.Empty;

                if (type.PropertyType.IsEnum)
                {
                    // TODO general culture switch from Template
                    if (propertyName == "Gender")
                    {
                        switch (value) {
                            case  "Male":  return "Männlich";
                            case "Female": return "Weiblich";
                            case "Other": return "Anders";
                            default: return String.Empty;
                        }
                    }
                }

                return value;
            }
        }


    }
}