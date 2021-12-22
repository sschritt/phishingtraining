using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Controllers.Base;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Models.PhishingMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhishingTraining.Web.Queries;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhishingTraining.Web.Models.Survey;
using PhishingTraining.Web.Helpers;

namespace PhishingTraining.Web.Controllers
{
    public class FeedbackController : ApplicationControllerBase
    {
        protected IConfiguration Configuration { get; }
        public object SurveyModel { get; private set; }

        public FeedbackController(IConfiguration configuration, IStringLocalizer<SharedResources> localizer, ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<CampaignController> logger) : base(localizer, dbContext, mapper, userManager, logger)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> Click(Guid id)
        {
            var phishingMessage = await GetPhishingMessagesByIdAsync(id);

            if (phishingMessage.TimeSent != null && phishingMessage.TimeSent != DateTimeOffset.MinValue)
            {
                if (phishingMessage.ClickDate == null)
                {
                    phishingMessage.ClickDate = DateTimeOffset.Now;
                    DbContext.SaveChanges();
                }

                if (phishingMessage.SurveyIsEmpty)
                {
                    SurveyModel survey = new SurveyModel();
                    survey.PhishingMessage = Mapper.Map<PhishingMessageModel>(phishingMessage);
                    return View("Survey", survey);
                }
            }
            else
            {
                ViewData["Title"] = "Test/Not sent yet";
            }

            return View(Mapper.Map<PhishingMessageModel>(phishingMessage));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Survey(SurveyModel model)
        {
            var phishingMessage = await GetPhishingMessagesByIdAsync(model.PhishingMessage.Id);
            if (phishingMessage.SurveyIsEmpty)
            {
                phishingMessage.ClickActivity = string.Join(",", model.SelectedActivities);
                phishingMessage.ClickLocation = string.Join(",", model.SelectedLocations);
                phishingMessage.ClickCompany = string.Join(",", model.SelectedCompanies);
                await DbContext.SaveChangesAsync();
            }
            return RedirectToAction("ThankYou");
        }
        [HttpGet]
        public IActionResult ThankYou()
        {
            return View();
        }

        private async Task<PhishingMessage> GetPhishingMessagesByIdAsync(Guid id) => await DbContext.Query<PhishingMessage>().IncludeCampaign().IncludeTemplate().SingleAsync(x => x.Id == id);
    }
}