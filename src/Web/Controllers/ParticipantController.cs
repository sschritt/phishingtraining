using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Controllers.Base;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Helpers.Security;
using PhishingTraining.Web.Models.Campaign;
using PhishingTraining.Web.Queries;

namespace PhishingTraining.Web.Controllers
{
    [Authorize(Policies.IsParticipant)]
    public class ParticipantController : ApplicationControllerBase
    {
        public ParticipantController(IStringLocalizer<SharedResources> localizer, ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<ParticipantController> logger) : base(localizer, dbContext, mapper, userManager, logger)
        {
        }

        [HttpGet]
        public async Task<IActionResult> JoinCampaign()
        {
            if (HttpContext.Request.Cookies.TryGetValue(JoinController.JoinCampaignCookieName, out var campaignIdString) && !string.IsNullOrWhiteSpace(campaignIdString))
            {
                var id = Guid.Parse(campaignIdString);
                HttpContext.Response.Cookies.Delete(JoinController.JoinCampaignCookieName);

                var campaign = await GetEntityByIdAsync(id);
                if (campaign.Participants.Contains(await GetApplicationUserAsync()))
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewData["Title"] = Localizer["Campaign \"{0}\"", campaign.Name];
                return View(new JoinCampaignModel(id));
            }

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).ToControllerName());
        }

        [HttpPost]
        public async Task<IActionResult> JoinCampaign(JoinCampaignModel model)
        {
            var campaign = await GetEntityByIdAsync(model.CampaignId);
            var user = await GetApplicationUserAsync();
            if (!campaign.Participants.Contains(user))
            {
                campaign.Participants.Add(user);
            }

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).ToControllerName());
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Overview));
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var campaigns = await DbContext.Query<ApplicationUser>().MyParticipatingCampaigns(GetUserId()).ToListAsync();
            var models = Mapper.Map<List<CampaignDisplayModel>>(campaigns);
            return View(models);
        }

        private async Task<Campaign> GetEntityByIdAsync(Guid id) => await DbContext.Query<Campaign>().IncludeParticipants().SingleAsync(campaign => campaign.Id == id);
    }
}