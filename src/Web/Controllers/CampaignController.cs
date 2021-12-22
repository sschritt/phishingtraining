using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Controllers.Base;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Helpers.Security;
using PhishingTraining.Web.Models.Campaign;
using PhishingTraining.Web.Queries;
using PhishingTraining.Web.Models.PhishingTemplate;
using PhishingTraining.Web.Models.Participant;
using PhishingTraining.Web.Models.PhishingMessage;
using PhishingTraining.Web.Services;
using PhishingTraining.Web.Services.Interfaces;
using PhishingTraining.Web.Models.Manager;

namespace PhishingTraining.Web.Controllers
{
    [Authorize(Policies.CampaignManagement)]
    public class CampaignController : ApplicationControllerBase
    {
        private IConfiguration Configuration { get; }
        private IMessageGenerator MessageGenerator { get; }
        private IReportService ReportService { get; }
        private IApplicationEmailSender EmailSender { get; }
        private IApplicationSmsSender SmsSender { get; }

        public CampaignController(IConfiguration configuration, IApplicationEmailSender emailSender, IApplicationSmsSender smsSender, IStringLocalizer<SharedResources> localizer, ApplicationDbContext dbContext, IMapper mapper, IMessageGenerator messageGenerator,  UserManager<ApplicationUser> userManager, ILogger<CampaignController> logger, IReportService reportService) : base(localizer, dbContext, mapper, userManager, logger)
        {
            Configuration = configuration;
            MessageGenerator = messageGenerator;
            ReportService = reportService;
            EmailSender = emailSender;
            SmsSender = smsSender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Overview));
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var userQuery = DbContext.Query<ApplicationUser>();
            var campaigns = await userQuery.MyManagedCampaigns(GetUserId()).IncludeParticipants().IncludeTemplates().ToListAsync();
            var models = Mapper.Map<List<CampaignDisplayModel>>(campaigns);
            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = Localizer["Create Campaign"];
            return CreateAndEditView(null);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);

            ViewData["Title"] = Localizer["Edit Campaign \"{0}\"", entity.Name];
            return CreateAndEditView(Mapper.Map<CampaignEditModel>(entity));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var campaignEntity = await GetEntityAndSetViewData(id);
            return View(Mapper.Map<CampaignDisplayModel>(campaignEntity));
        }

        [HttpGet]
        public async Task<IActionResult> Participants(Guid id)
        {
            var campaignEntity = await GetEntityAndSetViewData(id);
            var participants = campaignEntity.Participants;
            var models = Mapper.Map<List<ParticipantDisplayModel>>(participants);

            models.ForEach(x => x.CampaignId = campaignEntity.Id);

            return ParticipantsView(models);
        }

        [HttpGet]
        public async Task<IActionResult> MessageDetails(Guid id)
        {
            var messageEntity = await GetPhishingMessageWithUserWithTemplateByIdAsync(id);
            var model = Mapper.Map<PhishingMessageModel>(messageEntity);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Managers(Guid id)
        {
            var campaignEntity = await GetEntityAndSetViewData(id);
            var managerEntities = campaignEntity.Managers;

            var managers = Mapper.Map<List<ManagerDisplayModel>>(managerEntities);
            managers.ForEach(x => x.CampaignId = campaignEntity.Id);

            ViewData["CampaignId"] = campaignEntity.Id;
            return View(managers);
        }

        [HttpGet]
        public async Task<IActionResult> Status(Guid id)
        {
            var campaignEntity = await GetEntityAndSetViewData(id);
            var campaingModel = Mapper.Map<CampaignStatusModel>(campaignEntity);
            var phishingSentMessages = await GetSentPhishingMessagesByCampaignIdAsync(id);
            var phishingUnsentMessages = await GetUnsentPhishingMessagesByCampaignIdAsync(id);
            campaingModel.MessagesSent = phishingSentMessages.Count(message => message.TimeSent != null && message.TimeSent != DateTimeOffset.MinValue);
            campaingModel.MessagesClicked = phishingSentMessages.Count(message => message.ClickDate != null && message.ClickDate != DateTimeOffset.MinValue);
            campaingModel.MessagesPlanned = phishingUnsentMessages.Count();
            campaingModel.ParticipantsCount = campaignEntity.Participants.Count();
            campaingModel.TemplateUsageCount = campaignEntity.TemplateUsage.Count();

            return View(campaingModel);
        }

        [HttpGet]
        public async Task<IActionResult> PlannedMessages(Guid id)
        {
            var campaignEntity = await GetEntityAndSetViewData(id);
            var campaignMessagesModel = Mapper.Map<CampaignMessagesModel>(campaignEntity);
            var phishingMessages = await GetUnsentPhishingMessagesByCampaignIdAsync(id);
            var phishingMessagesModel = Mapper.Map<List<PhishingMessageModel>>(phishingMessages);
            campaignMessagesModel.Templates = phishingMessagesModel;

            return PlannedMessagesView(campaignMessagesModel);
        }

        [HttpGet]
        public async Task<IActionResult> SentMessages(Guid id)
        {
            var campaignEntity = await GetEntityAndSetViewData(id);
            var campaignMessagesModel = Mapper.Map<CampaignMessagesModel>(campaignEntity);
            var phishingMessages = await GetSentPhishingMessagesByCampaignIdAsync(id);
            var phishingMessagesModel = Mapper.Map<List<PhishingMessageModel>>(phishingMessages);
            campaignMessagesModel.Templates = phishingMessagesModel;

            return View("SentMessages", campaignMessagesModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);

            ViewData["Title"] = Localizer["Delete Campaign"];
            return View(Mapper.Map<CampaignDisplayModel>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);

            DbContext.Remove(entity);

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<PhishingTemplate>> GlobalTemplates() => await DbContext.Query<PhishingTemplate>().ToListAsync();

        [HttpGet]
        public async Task<IActionResult> Templates(Guid id)
        {
            var campaignEntity = await GetEntityAndSetViewData(id);
            var campaignTemplatesIds = campaignEntity.TemplateUsage.Select(x => x.Id);
            var templates = await GlobalTemplates();

            var models = Mapper.Map<List<PhishingTemplateCampaignDisplayModel>>(templates);
            models.ForEach(x => x.CampaignId = campaignEntity.Id);

            foreach (var campaignTemplateId in campaignTemplatesIds)
            {
                models.Where(temp => temp.Id == campaignTemplateId).First().IsChecked = true;
            }

            foreach (var model in models)
            {
                model.HasTemplateFile = TemplateHelper.HasTemplateFile(model.Id, Configuration.TemplateDataStoragePath());
                model.HasPlainTemplateFile = TemplateHelper.HasPlainTemplateFile(model.Id, Configuration.TemplateDataStoragePath());
            }

            return TemplatesView(models);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(Guid id)
        {
            //TODO generate messages - input: particpants and each campaign templates

            //Removing all sent messages first 
            var oldPhishingMessages = await GetUnsentPhishingMessagesByCampaignIdAsync(id);
            foreach (var phishingMessage in oldPhishingMessages)
            {
                if (phishingMessage.TimeSent == null)
                    DbContext.Remove(phishingMessage);
            }

            await MessageGenerator.GenerateMessages(id);

            return RedirectToAction("PlannedMessages", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> AddTemplate(Guid id, Guid campaign)
        {
            var campaignEntity = await GetEntityByIdAsync(campaign);
            var templateEntity = await GetTemplateEntityByIdAsync(id);

            campaignEntity.TemplateUsage.Add(templateEntity);
            DbContext.SaveChanges();

            return RedirectToAction("Templates", new { id = campaign });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTemplate(Guid id, Guid campaign)
        {
            var campaignEntity = await GetEntityByIdAsync(campaign);
            var templateEntity = await GetTemplateEntityByIdAsync(id);

            campaignEntity.TemplateUsage.Remove(templateEntity);
            DbContext.SaveChanges();

            return RedirectToAction("Templates", new { id = campaign });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlannedMessage(Guid messageId, Guid campaignId)
        {
            var phishingMessage = await GetPhishingMessagesByIdAsync(messageId);
            DbContext.Remove(phishingMessage);
            await DbContext.SaveChangesAsync();

            return RedirectToAction("PlannedMessages", new { id = campaignId });
        }

        [HttpPost]
        public async Task<IActionResult> SendTest(Guid messageId, Guid campaignId)
        {
            var phishingMessage = await DbContext.Query<PhishingMessage>().WhereId(messageId).IncludeTemplate().SingleAsync();
            var currentUser = await GetApplicationUserAsync();

            switch (phishingMessage.PhishingTemplate.Type)
            {
                case Enums.TemplateType.Email:
                    EmailSender.SendTestPhishingMessage(messageId, currentUser.Email);
                    break;
                case Enums.TemplateType.Sms:
                    SmsSender.SendTestPhishingMessage(messageId, currentUser.PhoneNumber);
                    break;
            }

            return RedirectToAction("PlannedMessages", new { id = campaignId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveParticipant(Guid campaign, string userName)
        {
            var campaignEntity = await GetEntityByIdAsync(campaign);
            var userEntity = DbContext.Query<ApplicationUser>().Where(user => user.UserName == userName).SingleOrDefault();

            campaignEntity.Participants.Remove(userEntity);
            //TODO necessary?
            DbContext.SaveChanges();

            return RedirectToAction("Participants", new { id = campaign });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveManager(Guid campaign, string userName)
        {
            var campaignEntity = await GetEntityByIdAsync(campaign);
            var userEntity = DbContext.Query<ApplicationUser>().Where(user => user.UserName == userName).SingleOrDefault();

            campaignEntity.Managers.Remove(userEntity);
            //TODO necessary?
            DbContext.SaveChanges();

            return RedirectToAction("Managers", new { id = campaign });
        }

        [HttpPost]
        public async Task<IActionResult> AddManager(Guid campaign, string userName)
        { 
            var campaignEntity = await GetEntityByIdAsync(campaign);
            var userEntity = DbContext.Query<ApplicationUser>().Where(user => user.UserName == userName).SingleOrDefault();

            if ((userEntity) != null && !campaignEntity.Managers.Any(x=>x.UserName == userName))
            {
                campaignEntity.Managers.Add(userEntity);
                DbContext.SaveChanges();
            }

            return RedirectToAction("Managers", new { id = campaign });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(CampaignEditModel model)
        {
            Campaign entity;
            if (model.Id != null)
            {
                entity = await GetEntityByIdAsync(model.Id.Value);
                Mapper.Map(model, entity);

                DbContext.Update(entity);
            }
            else
            {
                model.Id = Guid.NewGuid();
                entity = Mapper.Map<Campaign>(model);

                var user = await GetApplicationUserAsync();
                if (!entity.Managers.Contains(user))
                {
                    entity.Managers.Add(user);
                }

                DbContext.Add(entity);
            }

            return RedirectToAction(nameof(Index));
        }

        private IActionResult CreateAndEditView(CampaignEditModel model)
        {
            return View("CreateAndEdit", model);
        }

        private IActionResult ParticipantsView(List<ParticipantDisplayModel> model)
        {
            return View("Participants", model);
        }

        private IActionResult TemplatesView(List<PhishingTemplateCampaignDisplayModel> model)
        {
            return View("Templates", model);
        }

        private IActionResult PlannedMessagesView(CampaignMessagesModel model)
        {
            return View("PlannedMessages", model);
        }

        private async Task<Campaign> GetEntityAndSetViewData(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);

            ViewData["Title"] = entity.Name;
            ViewData["CampaignId"] = id;
            return entity;
        }

        [HttpGet]
        public IActionResult Anonymize()
        {
            throw new NotImplementedException();
        }
        
        [HttpGet]
        public async Task<IActionResult> CsvExport(Guid id)
        {
            var csvFile = await ReportService.GenerateCsvReportForCampaign(id);
            return File(csvFile, "text/csv", $"{id}_{DateTimeOffset.Now:yyyy-MM-dd-HH-mm}.csv");
        }

        private async Task<Campaign> GetEntityByIdAsync(Guid id) => await DbContext.Query<Campaign>().IncludeParticipants().IncludeManagers().IncludeTemplates().SingleAsync(campaign => campaign.Id == id);
        private async Task<Campaign> GetEntityWithMetadataByIdAsync(Guid id) => await DbContext.Query<Campaign>().IncludeParticipants().IncludeManagers().IncludeTemplates().SingleAsync(campaign => campaign.Id == id);
        private async Task<PhishingTemplate> GetTemplateEntityByIdAsync(Guid id) => await DbContext.Query<PhishingTemplate>().IncludeParameters().SingleAsync(template => template.Id == id);
        private async Task<PhishingMessage> GetPhishingMessagesByIdAsync(Guid id) => await DbContext.Query<PhishingMessage>().SingleAsync(x=>x.Id == id);
        private async Task<PhishingMessage> GetPhishingMessageWithUserWithTemplateByIdAsync(Guid id) => await DbContext.Query<PhishingMessage>().IncludeUser().IncludeTemplate().SingleAsync(x => x.Id == id);
        private async Task<IList<PhishingMessage>> GetPhishingMessagesByCampaignIdAsync(Guid id) => await DbContext.Query<PhishingMessage>().WhereCampaignId(id).IncludeUser().ToListAsync();
        private async Task<IList<PhishingMessage>> GetUnsentPhishingMessagesByCampaignIdAsync(Guid id) => await DbContext.Query<PhishingMessage>().WhereCampaignId(id).WhereUnsent().ToListAsync();
        private async Task<IList<PhishingMessage>> GetSentPhishingMessagesByCampaignIdAsync(Guid id) => await DbContext.Query<PhishingMessage>().WhereCampaignId(id).WhereSent().ToListAsync();
        
    }
}
