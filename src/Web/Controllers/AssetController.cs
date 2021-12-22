using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers;

namespace PhishingTraining.Web.Controllers
{
    //TODO Authorization missing
    public class AssetController : Controller
    {
        private ApplicationDbContext DbContext { get; }
        protected IConfiguration Configuration { get; }

        public AssetController(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            Configuration = configuration;
            DbContext = dbContext;
        }
        // GET
        public async Task<IActionResult> GetAsset(Guid? messageId, string asset)
        {
            if (messageId == null || String.IsNullOrWhiteSpace(asset))
                return BadRequest();
            
            var phishingMessage = await DbContext.Query<PhishingMessage>().FirstOrDefaultAsync(x => x.Id == messageId);
            var storagePath = Configuration.TemplateDataStoragePath();
            var templateId = phishingMessage.PhishingTemplateId.ToString();
            var assetPath = HttpUtility.UrlDecode(asset);

            var path = Path.Join(storagePath, templateId, assetPath);
            if (!System.IO.File.Exists(path))
                return BadRequest();
            var contentType = "application/octet-stream"; //default
            var extension = new FileInfo(path).Extension;
            switch (extension.ToUpperInvariant())
            {
                case ".PNG": 
                    contentType = "image/png"; 
                    break;
                case ".JPEG": 
                case ".JPG": 
                case ".JPE": 
                    contentType = "image/jpeg"; 
                    break;
                case ".GIF": 
                    contentType = "image/gif"; 
                    break;
            }
            if (phishingMessage.TimeSent != null && phishingMessage.TimeSent != DateTimeOffset.MinValue && 
                (phishingMessage.FetchImageDate == null || phishingMessage.FetchImageDate == DateTimeOffset.MinValue))
            {
                phishingMessage.FetchImageDate = DateTimeOffset.Now;
                await DbContext.SaveChangesAsync();
            }

            return new FileContentResult(System.IO.File.ReadAllBytes(path), contentType);
        }
    }
}