using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Controllers.Base;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers.Security;
using PhishingTraining.Web.Models.PhishingTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.IO;
using PhishingTraining.Web.Queries;

namespace PhishingTraining.Web.Controllers
{
    [Authorize(Policies.TemplateManagement)]
    public class TemplateController : ApplicationControllerBase
    {
        protected IConfiguration Configuration { get; }

        public TemplateController(IStringLocalizer<SharedResources> localizer, ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<TemplateController> logger, IConfiguration configuration) : base(localizer, dbContext, mapper, userManager, logger)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userQuery = DbContext.Query<ApplicationUser>();
            var templates = await GlobalTemplates();
            var models = Mapper.Map<List<PhishingTemplateDisplayModel>>(templates);
            foreach (var model in models)
            {
                model.HasTemplateFile = TemplateHelper.HasTemplateFile(model.Id, Configuration.TemplateDataStoragePath());
                model.HasPlainTemplateFile = TemplateHelper.HasPlainTemplateFile(model.Id, Configuration.TemplateDataStoragePath());
            }
            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = Localizer["Create Template"];
            return CreateAndEditView(null);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);

            ViewData["Title"] = Localizer["Edit Template"];
            return CreateAndEditView(Mapper.Map<PhishingTemplateModel>(entity));
        }

        [HttpGet]
        public async Task<IActionResult> Copy(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);

            ModelState.Clear();

            var newEntity = new PhishingTemplate()
            {
                Id = Guid.Empty,
                Description = entity.Description,
                Name = String.Format($"Copy - {entity.Name}"),
                SenderAddress = entity.SenderAddress,
                SenderName = entity.SenderName,
                Type = entity.Type,
                Parameters = entity.Parameters,
            };

            ViewData["Title"] = Localizer["Copy Template"];
            var model = Mapper.Map<PhishingTemplateModel>(newEntity);
            model.CopyTemplateFilesFromTemplateId = id;
            return CreateAndEditView(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAndEdit(PhishingTemplateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.From > model.To)
            {
                ModelState.AddModelError("From", "From must be earlier than To");
                ModelState.AddModelError("To", "To must be later than From");
                return View(model);
            }

            if (model.PlainTemplateFile != null)
            {
                var extension = Path.GetExtension(model.PlainTemplateFile.FileName);
                if (!extension.ToLower().Equals(".txt"))
                {
                    ModelState.AddModelError("PlainTemplateFile", "Plain Template File requires a .txt");
                }
            }

            if (model.TemplateFile != null)
            {
                var extension = Path.GetExtension(model.TemplateFile.FileName);
                if (!extension.ToLower().Equals(".zip"))
                {
                    ModelState.AddModelError("TemplateFile", "HTML Template File requires a .zip");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            PhishingTemplate entity;

            if (model.Id != null && model.Id != Guid.Empty)
            {
                entity = await UpdateExistingTemplate(model);
            }
            else
            {
                entity = await CreateNewTemplate(model);
            }
            if (model.TemplateFile != null)
            {
                await UpdateHtmlTemplateFile(model.TemplateFile, entity);
            }
            if (model.PlainTemplateFile != null)
            {
                await UpdatePlainTemplateFile(model.PlainTemplateFile, entity);
            }
            else if (model.CopyTemplateFilesFromTemplateId.HasValue && model.CopyTemplateFilesFromTemplateId != Guid.Empty)
            {
                var filepath = Configuration.TemplateZipPath(model.CopyTemplateFilesFromTemplateId.Value);
                if (System.IO.File.Exists(filepath))
                {
                    using (var memStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(filepath)))
                    {
                        await UpdateHtmlTemplateFile(memStream, entity);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<PhishingTemplate> UpdateExistingTemplate(PhishingTemplateModel model)
        {
            var entity = await GetEntityByIdAsync(model.Id.Value);
            Mapper.Map(model, entity);
            DbContext.Update(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        private async Task<PhishingTemplate> CreateNewTemplate(PhishingTemplateModel model)
        {
            model.Id = Guid.NewGuid();
            var entity = Mapper.Map<PhishingTemplate>(model);
            DbContext.Add(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        private async Task UpdateHtmlTemplateFile(IFormFile templateFile, PhishingTemplate templateEntity)
        {
            await using var memStream = new MemoryStream();
            await templateFile.OpenReadStream().CopyToAsync(memStream);
            await UpdateHtmlTemplateFile(memStream, templateEntity);
        }

        private async Task UpdateHtmlTemplateFile(MemoryStream memStream, PhishingTemplate templateEntity)
        {
            //store file to filesystem, save zip as {templateId}.zip
            if (await IsValidTemplateZip(memStream))
            {
                var filepath = Configuration.TemplateZipPath(templateEntity.Id);
                var templateRootPath = Configuration.TemplateRootPath(templateEntity.Id);
                await using (var file = System.IO.File.Create(filepath))
                {
                    await file.WriteAsync(memStream.ToArray(), 0, (int)memStream.Length);
                }
                //clear old folder if exist
                if (Directory.Exists(templateRootPath))
                {
                    Directory.Delete(templateRootPath, true);
                }
                ZipFile.ExtractToDirectory(filepath, templateRootPath, true);
                var pathInfo = new DirectoryInfo(templateRootPath);
                var htmlFileInfo = pathInfo.GetFiles("*.html").First();
                var targetHtmlFilename = $"{templateRootPath}/{templateEntity.Id}.html";
                htmlFileInfo.MoveTo(targetHtmlFilename);

                // TODO remove in the future - directly access with ID in file system
                templateEntity.HtmlTemplateFilename = htmlFileInfo.Name;
                await DbContext.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("TemplateFile", "Given TemplateFile is not a valid zip file");
            }
        }

        private async Task UpdatePlainTemplateFile(IFormFile templateFile, PhishingTemplate templateEntity)
        {
            await using var memStream = new MemoryStream();
            await templateFile.OpenReadStream().CopyToAsync(memStream);
            await UpdatePlainTemplateFile(memStream, templateEntity);
        }

        private async Task UpdatePlainTemplateFile(MemoryStream memStream, PhishingTemplate templateEntity)
        {
            var templateFilename = $"{templateEntity.Id}.txt";
            var filepath = Configuration.TemplateDataStoragePath() + "/" + templateFilename;

            await using (var file = System.IO.File.Create(filepath))
            {
                await file.WriteAsync(memStream.ToArray(), 0, (int)memStream.Length);
            }

            // TODO remove in the future - directly access with ID in file system
            templateEntity.PlainTemplateFilename = templateFilename;
            await DbContext.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);
            ViewData["Title"] = Localizer["Delete Template"];
            return View(Mapper.Map<PhishingTemplateDisplayModel>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);
            var unsentMessages = await GetUnsentPhishingMessagesByTemplateId(id);
            DeleteTemplateZipIfExist(entity);
            DeleteTemplatePlainIfExist(entity);
            DbContext.Remove(entity);
            DbContext.RemoveRange(unsentMessages);
            return RedirectToAction(nameof(Index));
        }

        private void DeleteTemplateZipIfExist(PhishingTemplate phishingTemplate)
        {
            var filePath = Configuration.TemplateZipPath(phishingTemplate.Id);
            if (System.IO.File.Exists(filePath))
            {
                var folderPath = Configuration.TemplateRootPath(phishingTemplate.Id);
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);
                System.IO.File.Delete(filePath);
            }
        }

        private void DeleteTemplatePlainIfExist(PhishingTemplate phishingTemplate)
        {
            var templateFilename = $"{phishingTemplate.Id}.txt";
            var filepath = Configuration.TemplateDataStoragePath() + "/" + templateFilename;

            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);
            ViewData["HasTemplate"] = TemplateHelper.HasTemplateFile(id, Configuration.TemplateDataStoragePath());
            ViewData["HasPlainTemplate"] = TemplateHelper.HasPlainTemplateFile(id, Configuration.TemplateDataStoragePath());
            ViewData["Title"] = Localizer["Template Details"];
            return View(Mapper.Map<PhishingTemplateDisplayModel>(entity));
        }

        [HttpGet]
        public IActionResult GetTemplateFile(Guid id)
        {
            var filepath = Configuration.TemplateZipPath(id);
            if (System.IO.File.Exists(filepath))
            {
                return new PhysicalFileResult(filepath, "application/zip");
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult GetPlainTemplateFile(Guid id)
        {
            var templateFilename = $"{id}.txt";
            var filepath = Configuration.TemplateDataStoragePath() + "/" + templateFilename;

            if (System.IO.File.Exists(filepath))
            {
                return new PhysicalFileResult(filepath, "text/plain");
            }
            return BadRequest();
        }

        private IActionResult CreateAndEditView(PhishingTemplateModel model)
        {
            ViewData["HasTemplate"] = TemplateHelper.HasTemplateFile(model?.Id ?? Guid.Empty, Configuration.TemplateDataStoragePath());
            ViewData["HasPlainTemplate"] = TemplateHelper.HasPlainTemplateFile(model?.Id ?? Guid.Empty, Configuration.TemplateDataStoragePath());
            return View("CreateAndEdit", model);
        }

        private async Task<bool> IsValidTemplateZip(MemoryStream memoryStream)
        {
            var path = Configuration.TemplateDataStoragePath();
            var tempFilepath = $"{path}/{Guid.NewGuid()}.zip";
            await using (var file = System.IO.File.Create(tempFilepath))
            {
                await file.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            }

            var isValidTemplateZip = false;
            try
            {
                using (var zipFile = ZipFile.OpenRead(tempFilepath))
                {
                    var entries = zipFile.Entries;
                    var htmlfiles = zipFile.Entries.Where(x => x.Name.EndsWith(".html") && x.FullName == x.Name);
                    if (htmlfiles.Count() == 1)
                        isValidTemplateZip = true;
                }
            }
            catch (InvalidDataException)
            {
                isValidTemplateZip = false;
            }
            finally
            {
                System.IO.File.Delete(tempFilepath);
            }
            return isValidTemplateZip;
        }

        private Task<PhishingTemplate> GetEntityByIdAsync(Guid id) => DbContext.Query<PhishingTemplate>().SingleAsync(template => template.Id == id);
        private Task<List<PhishingTemplate>> GlobalTemplates() => DbContext.Query<PhishingTemplate>().ToListAsync();
        private Task<List<PhishingMessage>> GetUnsentPhishingMessagesByTemplateId(Guid id) =>
            DbContext.Set<PhishingMessage>().WhereUnsent().WhereTemplateId(id).ToListAsync();
    }
}
