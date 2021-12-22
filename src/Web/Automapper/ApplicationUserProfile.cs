using AutoMapper;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Models.Manager;
using PhishingTraining.Web.Models.Participant;

namespace PhishingTraining.Web.Automapper
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, ParticipantDisplayModel>();
            CreateMap<ApplicationUser, ParticipantStatusModel>();
            CreateMap<ApplicationUser, ManagerDisplayModel>();
        }
    }
}