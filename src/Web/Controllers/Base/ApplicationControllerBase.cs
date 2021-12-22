using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;

namespace PhishingTraining.Web.Controllers.Base
{
    public class ApplicationControllerBase : Controller
    {
        public ApplicationControllerBase(IStringLocalizer<SharedResources> localizer, ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            Localizer = localizer;
            DbContext = dbContext;
            Mapper = mapper;
            UserManager = userManager;
            Logger = logger;
        }

        protected IStringLocalizer Localizer { get; }
        protected ApplicationDbContext DbContext { get; }
        protected IMapper Mapper { get; }
        protected UserManager<ApplicationUser> UserManager { get; }
        protected ILogger Logger { get; }
       
        protected Guid GetUserId() => Guid.Parse(UserManager.GetUserId(User));
        protected async Task<ApplicationUser> GetApplicationUserAsync() => await UserManager.GetUserAsync(User);
    }
}