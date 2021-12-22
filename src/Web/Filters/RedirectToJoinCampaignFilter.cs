using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PhishingTraining.Web.Controllers;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Helpers.Security;

namespace PhishingTraining.Web.Filters
{
    public class RedirectToJoinCampaignFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Cookies.ContainsKey(JoinController.JoinCampaignCookieName) && context.HttpContext.User.IsInRole(Roles.Participant))
            {
                if (!nameof(ParticipantController).ToControllerName().Equals(context.RouteData.Values["controller"]) &&
                    !nameof(ParticipantController.JoinCampaign).Equals(context.RouteData.Values["action"]))
                {
                    context.Result = new RedirectToActionResult(nameof(ParticipantController.JoinCampaign),
                        nameof(ParticipantController).ToControllerName(), new { });
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //nothing
        }
    }
}