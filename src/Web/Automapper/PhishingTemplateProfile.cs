using System;
using AutoMapper;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Models.PhishingTemplate;

namespace PhishingTraining.Web.Automapper
{
    public class PhishingTemplateProfile : Profile
    {
        public PhishingTemplateProfile()
        {
            CreateMap<PhishingTemplate, PhishingTemplateModel>(); 
            CreateMap<PhishingTemplate, PhishingTemplateDisplayModel>();
            CreateMap<PhishingTemplateModel, PhishingTemplate>()
                .ForMember(template => template.Id, expression => expression.NullSubstitute(Guid.NewGuid()));

            CreateMap<PhishingTemplate, PhishingTemplateCampaignDisplayModel>();
        }
    }
}