using AutoMapper;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Models.PhishingMessage;

namespace PhishingTraining.Web.Automapper
{
    public class PhishingMessageProfile : Profile
    {
        public PhishingMessageProfile()
        {
            CreateMap<PhishingMessage, PhishingMessageModel>();
        }
    }
}