using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Controllers.Base;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers.Security;
using PhishingTraining.Web.Models.Administration;

namespace PhishingTraining.Web.Controllers
{
    [Authorize(Policies.IsAdministrator)]
    public class AdministrationController : ApplicationControllerBase
    {
        //private RoleManager<ApplicationRole> RoleManager { get; }

        public AdministrationController(IStringLocalizer<SharedResources> localizer, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IMapper mapper, ILogger<AdministrationController> logger) : base(localizer, dbContext, mapper, userManager, logger)
        {
        }

        // GET: Administration
        //public async Task<IActionResult> Index()
        //{
        //    return View(await UserManager.Users.ToListAsync());
        //}

        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var userEntities = await UserManager.Users.ToListAsync();
            var users = Mapper.Map<List<ApplicationUserModel>>(userEntities);

            await SetRoleFlags(userEntities, users);

            return View(users);
        }

        private async Task SetRoleFlags(List<ApplicationUser> userEntities, List<ApplicationUserModel> users)
        {
            foreach (var userEntity in userEntities)
            {
                if (await UserManager.IsInRoleAsync(userEntity, Roles.Admin))
                {
                    users.Single(x => x.UserName == userEntity.UserName).IsAdministrator = true;
                }

                if (await UserManager.IsInRoleAsync(userEntity, Roles.Manager))
                {
                    users.Single(x => x.UserName == userEntity.UserName).IsManager = true;
                }
            }
        }

        [HttpGet]
        [Route("Administration/ApplicationUserEdit/{userName}")]
        public async Task<IActionResult> ApplicationUserEdit(string userName)
        {
            ViewData["Title"] = Localizer["Edit User"];
            var userEntity = await UserManager.FindByNameAsync(userName);
            var user = Mapper.Map<ApplicationUserModel>(userEntity);

            if (await UserManager.IsInRoleAsync(userEntity, Roles.Admin))
            {
                user.IsAdministrator = true;
            }

            if (await UserManager.IsInRoleAsync(userEntity, Roles.Manager))
            {
                user.IsManager = true;
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ApplicationUserEdit(ApplicationUserModel user)
        {
            var userEntity = await UserManager.FindByNameAsync(user.UserName);

            await UpdateUserRole(userEntity, Roles.Admin, user.IsAdministrator);
            await UpdateUserRole(userEntity, Roles.Manager, user.IsManager);

            return RedirectToAction(nameof(ListUsers));
        }

        private async Task UpdateUserRole(ApplicationUser user, string role, bool hasRole)
        {
            if (hasRole)
            {
                await AddUserToRole(user, role);
            }
            else
            {
                await RemoveUserRole(user, role);
            }
        }

        private async Task AddUserToRole(ApplicationUser userEntity, string role)
        {
            if (!await UserManager.IsInRoleAsync(userEntity, role))
            {
                var result = await UserManager.AddToRoleAsync(userEntity, role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Couldn't add role {role} to: {userEntity?.UserName}");
                }
            }
        }

        private async Task RemoveUserRole(ApplicationUser userEntity, string role)
        {
            if (await UserManager.IsInRoleAsync(userEntity, role))
            {
                var result = await UserManager.RemoveFromRoleAsync(userEntity, role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Couldn't add role {role} to: {userEntity?.UserName}");
                }
            }
        }
    }
}
