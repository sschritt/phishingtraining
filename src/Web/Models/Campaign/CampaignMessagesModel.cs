using PhishingTraining.Web.Models.PhishingMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Campaign
{
    public class CampaignMessagesModel
    {
        public Guid Id { get; set; }

        public IList<PhishingMessageModel> Templates { get; set; } = new List<PhishingMessageModel>();
    }
}