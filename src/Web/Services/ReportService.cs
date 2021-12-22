using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Queries;
using PhishingTraining.Web.Services.Interfaces;

namespace PhishingTraining.Web.Services
{
    public class ReportService : ServiceBase, IReportService
    {
        public ReportService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        public async Task<byte[]> GenerateCsvReportForCampaign(Guid id)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };
            using (var memStream = new MemoryStream())
            {
                var phishingMessages = await DbContext.Query<PhishingMessage>().Include(x => x.User).WhereCampaignId(id).IncludeTemplate().ToListAsync();
                var campaign = await DbContext.Query<Campaign>().IncludeParticipants().IncludeManagers().IncludeTemplates().SingleAsync(campaign => campaign.Id == id);
                using (var writer = new StreamWriter(memStream))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, false))
                    {
                        var participantMessageCounter = phishingMessages.Select(x => x.UserId).Distinct()
                            .ToDictionary(x => x, x => 1);
                        var entries = phishingMessages.OrderBy(x => x.TimeSent).Select(x =>
                        {
                            return new
                            {
                                KampagneId = campaign.Id.ToString(),
                                KampagneName = campaign.Name,
                                Klasse = campaign.Class,
                                TeilnehmerId = x.UserId.ToString(),
                                Gender = x.User.Gender.ToString(),
                                Age = x.User.Birthdate.HasValue
                                    ? (DateTime.Today.Year - x.User.Birthdate.Value.Year)
                                    : 0,
                                PhishingTemplateId = x.PhishingTemplateId?.ToString(),
                                TemplateType = x.PhishingTemplate?.Type.ToString(),
                                TemplateDifficulty = x.PhishingTemplate?.Difficulty,
                                VersendetAm = x.TimeSent,
                                MailOpenedOn = x.FetchImageDate,
                                ClickedOn = x.ClickDate,
                                ClickLocation = x.ClickLocation,
                                ClickCompany = x.ClickCompany,
                                ClickActivity = x.ClickActivity,
                                DurchlaufendeNummer = participantMessageCounter[x.UserId]++
                            };
                        });
                        await csv.WriteRecordsAsync(entries);
                        await csv.FlushAsync();
                        await writer.FlushAsync();
                        var fileContent = memStream.ToArray();
                        return fileContent;
                    }
                }
            }
        }
    }
}