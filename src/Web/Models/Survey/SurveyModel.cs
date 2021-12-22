using Microsoft.AspNetCore.Mvc.Rendering;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Models.PhishingMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Survey
{
    public class SurveyModel
    {
        public PhishingMessageModel PhishingMessage { get; set; }

        public List<SelectListItem> Locations { get; } = new List<SelectListItem>();
        public List<SelectListItem> Activites { get; } = new List<SelectListItem>();
        public List<SelectListItem> Company { get; } = new List<SelectListItem>();

        public List<string> SelectedLocations { get; set; } = new List<string>();
        public List<string> SelectedActivities { get; set; } = new List<string>();
        public List<string> SelectedCompanies { get; set; } = new List<string>();

        public SurveyModel()
        {
            foreach(var location in SurveyHelper.Locations)
            {
                Locations.Add(new SelectListItem(location.Value, location.Key.ToString()));
            }

            foreach (var activity in SurveyHelper.Activites)
            {
                Activites.Add(new SelectListItem(activity.Value, activity.Key.ToString()));
            }

            foreach (var company in SurveyHelper.Company)
            {
                Company.Add(new SelectListItem(company.Value, company.Key.ToString()));
            }
        }
    }
}
