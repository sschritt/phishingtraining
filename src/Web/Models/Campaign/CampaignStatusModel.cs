using PhishingTraining.Web.Models.Participant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Campaign
{
    public class CampaignStatusModel : CampaignModel
    {
        public int MessagesSent { get; set; }
        public int MessagesClicked { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double MessagesClickedPercentage => MessagesSent > 0 ? ((double)MessagesClicked / MessagesSent) : 0;
        public int MessagesPlanned { get; set; }
        //public List<ParticipantDisplayModel> Participants { get; set; }
        public int ParticipantsCount { get; set; }
        public int TemplateUsageCount { get; set; }
    }
}
