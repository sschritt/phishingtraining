using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Campaign
{
    public class CampaignDisplayModel : CampaignModel
    {
        public int ParticipantsCount { get; set; }
        public int TemplateUsageCount { get; set; }
    }
}
