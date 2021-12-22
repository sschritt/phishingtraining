using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Participant
{
    public class ParticipantStatusModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MessagesSent { get; set; }
        public int MessagesClicked { get; set; }
        public int MessagesClickedPercentage { get; }
        public int MessagesPlanned { get; set; }
    }
}
