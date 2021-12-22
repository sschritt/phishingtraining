using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Campaign
{
    public class JoinCampaignModel
    {
        public Guid CampaignId { get; set; }

        public JoinCampaignModel(Guid campaignId)
        {
            CampaignId = campaignId;
        }

        public JoinCampaignModel()
        {
        }
    }
}
