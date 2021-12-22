using AutoMapper;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Models.Administration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Automapper
{
    public class AdministrationProfile : Profile
    {
        public AdministrationProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserModel>();
            CreateMap<ApplicationUserModel, ApplicationUser>();
        }
    }
}
