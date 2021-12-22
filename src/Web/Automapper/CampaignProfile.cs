using System;
using AutoMapper;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Models.Campaign;

namespace PhishingTraining.Web.Automapper
{
    public class CampaignProfile : Profile
    {
        public CampaignProfile()
        {
            CreateMap<Campaign, CampaignModel>();
            CreateMap<Campaign, CampaignDisplayModel>();
            CreateMap<Campaign, CampaignEditModel>();
            CreateMap<Campaign, CampaignStatusModel>();

            CreateMap<CampaignModel, Campaign>()
                .ForMember(campaign => campaign.Id, expression => expression.NullSubstitute(Guid.NewGuid()));
            
            CreateMap<CampaignEditModel, Campaign>()
              .ForMember(campaign => campaign.Id, expression => expression.NullSubstitute(Guid.NewGuid()));

            CreateMap<Campaign, CampaignMessagesModel>();
        }
    }
}
