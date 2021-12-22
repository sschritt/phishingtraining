using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Controllers.Base;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers;

namespace PhishingTraining.Web.Controllers
{
    public class JoinController : ApplicationControllerBase
    {
        public const string JoinCampaignCookieName = "JoinCampaign";
        public JoinController(IStringLocalizer<SharedResources> localizer, ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<JoinController> logger) : base(localizer, dbContext, mapper, userManager, logger)
        {
        }

        [HttpGet]
        public IActionResult Campaign(Guid id)
        {
            HttpContext.Response.Cookies.Append(JoinCampaignCookieName, id.ToString(),
                new CookieOptions { SameSite = SameSiteMode.Strict, HttpOnly = true, Secure = true });
            return RedirectToAction(nameof(ParticipantController.JoinCampaign),
                nameof(ParticipantController).ToControllerName());
        }
    }
}
