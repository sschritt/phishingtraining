using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using PhishingTraining.Web.Data;

namespace PhishingTraining.Web.Filters
{
    public class SaveChangesFilter : IAsyncActionFilter
    {
        private ApplicationDbContext PublicationContext { get; }

        public SaveChangesFilter(ApplicationDbContext publicationContext)
        {
            PublicationContext = publicationContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (next != null)
            {
                await next();
            }

            if (IsManipulatingHttpMethod(context.HttpContext))
            {
                await PublicationContext.SaveChangesAsync();
            }
        }

        private bool IsManipulatingHttpMethod(HttpContext httpContext)
        {
            return httpContext.Request.Method == HttpMethod.Delete.Method ||
                   httpContext.Request.Method == HttpMethod.Patch.Method ||
                   httpContext.Request.Method == HttpMethod.Post.Method ||
                   httpContext.Request.Method == HttpMethod.Put.Method;
        }
    }
}
