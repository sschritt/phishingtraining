using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.PhishingTemplate
{
    public class PhishingTemplateCampaignDisplayModel : PhishingTemplateDisplayModel
    {
        public Guid CampaignId { get; set; }
    }
}
