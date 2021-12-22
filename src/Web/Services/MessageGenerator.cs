using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Helpers;
using PhishingTraining.Web.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using PhishingTraining.Web.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace PhishingTraining.Web.Services
{
    public class MessageGenerator : ServiceBase, IMessageGenerator
    {
        private ILogger<MessageGenerator> Logger { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IActionContextAccessor ActionContextAccessor { get; }
        private IUrlHelperFactory UrlHelperFactory { get; }
        private IStringLocalizer Localizer { get; }
        private Random Randomizer { get; } = new Random();
        private IDictionary<Guid, TemplateTexts> bodyByTemplateId { get; set; } = new Dictionary<Guid, TemplateTexts>();
        private IList<String> UserMetadatas { get; set; } = new List<String>() { "UserName", "FirstName",
            "LastName", "MotherFirstName", "FatherFirstName", "PetName", "Birthdate", "Street", "City",
            "Country", "Gender", "InstagramUser", "FacebookUser", "TikTokUser",   "RealData", "PhoneOs",  "ComputerOs", "PhoneProvider"};
        private Campaign Campaign { get; set; }

        private async Task<Campaign> GetEntityByIdAsync(Guid id) => await DbContext.Query<Campaign>().IncludeParticipants().IncludeManagers().IncludeTemplates().SingleAsync(campaign => campaign.Id == id);

        public MessageGenerator(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, ILogger<MessageGenerator> logger, IStringLocalizer<SharedResources> localizer) : base(serviceProvider)
        {
            HttpContextAccessor = httpContextAccessor;
            ActionContextAccessor = actionContextAccessor;
            UrlHelperFactory = urlHelperFactory;
            Localizer = localizer;
            Logger = logger;
        }

        public async Task GenerateMessages(Guid id)
        {
            Campaign = await GetEntityByIdAsync(id);
            //UserMetadatas = await DbContext.Query<UserMetadata>().ToListAsync();

            Logger.LogInformation(string.Format($"Generate Templates: Campaign: {Campaign.Name} - {DateTime.Now}"));

            var campaignMessagesList = new Dictionary<ApplicationUser, IList<PhishingMessage>>();
            bodyByTemplateId = await LoadTemplateLookup(Campaign.TemplateUsage);

            // Individual Mails
            GenerateIndividualMessagesForEachParticipant(campaignMessagesList, Campaign.NumberOfEMailMessagesPerParticipant, Enums.TemplateType.Email);

            // Group Mails - 10% in addtion to target Mail messages
            GenerateGroupEmailMessages(campaignMessagesList);

            // Individual SMS
            GenerateIndividualMessagesForEachParticipant(campaignMessagesList, Campaign.NumberOfEMailMessagesPerParticipant, Enums.TemplateType.Sms);

            // Group Sms - 10% in addtion to target Mail messages
            GenerateGroupSmsMessages(campaignMessagesList);

            // Individual Facebook
            GenerateIndividualMessagesForEachParticipant(campaignMessagesList, Campaign.NumberOfFacebookMessagesPerParticipant, Enums.TemplateType.Facebook);

            // Individual Instagram
            GenerateIndividualMessagesForEachParticipant(campaignMessagesList, Campaign.NumberOfInstagramMessagesPerParticipant, Enums.TemplateType.Instagram);

            // Individual TikTok
            GenerateIndividualMessagesForEachParticipant(campaignMessagesList, Campaign.NumberOfTikTokMessagesPerParticipant, Enums.TemplateType.TikTok);


            foreach (var participantMessages in campaignMessagesList)
            {
                foreach (var message in participantMessages.Value)
                {
                    await DbContext.AddAsync(message);
                }
            }
        }

        //private void GenerateIndividualEmailMessagesForEachParticipant(Dictionary<ApplicationUser, IList<PhishingMessage>> campaignMessagesList)
        //{
        //    var emailIndividualTemplates = Campaign.TemplateUsage.Where(x => x.Type == Enums.TemplateType.Email && x.SendType == Enums.TemplateSendType.Individual).ToList();
        //    GenerateIndividualMessagesForEachParticipant(campaignMessagesList, emailIndividualTemplates, Campaign.NumberOfEMailMessagesPerParticipant);
        //}
        //private void GenerateIndividualSmsMessagesForEachParticipant(Dictionary<ApplicationUser, IList<PhishingMessage>> campaignMessagesList)
        //{
        //    var smsIndividualTemplates = Campaign.TemplateUsage.Where(x => x.Type == Enums.TemplateType.Sms && x.SendType == Enums.TemplateSendType.Individual).ToList();
        //    GenerateIndividualMessagesForEachParticipant(campaignMessagesList, smsIndividualTemplates, Campaign.NumberOfSMSMessagesPerParticipant);
        //}
        //private void GenerateIndividualInstagramMessagesForEachParticipant(Dictionary<ApplicationUser, IList<PhishingMessage>> campaignMessagesList)
        //{
        //    var instagramIndividualTemplates = Campaign.TemplateUsage.Where(x => x.Type == Enums.TemplateType.Instagram && x.SendType == Enums.TemplateSendType.Individual).ToList();
        //    GenerateIndividualMessagesForEachParticipant(campaignMessagesList, instagramIndividualTemplates, Campaign.NumberOfSMSMessagesPerParticipant);
        //}

        private void GenerateIndividualMessagesForEachParticipant(Dictionary<ApplicationUser, IList<PhishingMessage>> campaignMessagesList, int numberOfMessages, Enums.TemplateType messageType)
        {
            var individualTemplates = Campaign.TemplateUsage.Where(x => x.Type == messageType && x.SendType == Enums.TemplateSendType.Individual).ToList();

            foreach (var participant in Campaign.Participants)
            {
                Logger.LogInformation(string.Format($"Generate individual message: Participant: {participant.UserName}"));
                var participantMessagesList = new List<PhishingMessage>();
                var randomOrderedTemplates = individualTemplates.OrderBy(x => Randomizer.Next()).ToList();
                GenerateIndividualMessages(numberOfMessages, participant, participantMessagesList, randomOrderedTemplates);

                Logger.LogInformation(string.Format($"Finished individual message: {participantMessagesList.Count}"));
                if (campaignMessagesList.ContainsKey(participant))
                {
                    foreach (var participantMessage in participantMessagesList)
                    {
                        campaignMessagesList[participant].Add(participantMessage);
                    }
                }
                else
                {
                    campaignMessagesList.Add(participant, participantMessagesList);
                }
            }
        }
        private void GenerateIndividualMessages(int messagesPerParticipant, ApplicationUser participant, IList<PhishingMessage> participantMessagesList, List<PhishingTemplate> emailTemplatesRandomOrder)
        {
            int generatedMessages = 0;

            foreach (var template in emailTemplatesRandomOrder)
            {
                if (generatedMessages >= messagesPerParticipant)
                    break;

                if (!ValidateEmptyFields(template))
                    continue;

                var (templateStart, templateEnd) = InitializeTemplateDates(template);

                if (!ValidateTemplateTimeRange(template, templateStart, templateEnd))
                    continue;

                var timeToSend = RandomSendTime(templateStart, templateEnd, template.FromTimeOfDay, template.ToTimeOfDay);

                var message = InitializeNewPhishingMessage(participant, template, timeToSend);

                switch (template.Type)
                {
                    case Enums.TemplateType.Email:
                        PrepareEmailContent(participant, template, message);
                        break;
                    case Enums.TemplateType.Sms:
                        PrepareSmsContent(participant, template, message);
                        break;
                    default:
                        PrepareDummyContent(participant, template, message);
                        break;
                }

                participantMessagesList.Add(message);
                generatedMessages++;

                Logger.LogInformation(string.Format($"Added message Id: {message.Id} Subject: {message.Subject}"));
            }
        }

        private void GenerateGroupEmailMessages(Dictionary<ApplicationUser, IList<PhishingMessage>> campaignMessagesList)
        {
            var emailGroupTemplates = Campaign.TemplateUsage.Where(x => x.Type == Enums.TemplateType.Email && x.SendType == Enums.TemplateSendType.Group).ToList();
            var groupEmails = Math.Ceiling(Campaign.NumberOfEMailMessagesPerParticipant * 0.1);
            var groupEmailTemplatesRandomOrder = emailGroupTemplates.OrderBy(x => Randomizer.Next()).ToList();
            GenerateGroupMessages(campaignMessagesList, groupEmails, groupEmailTemplatesRandomOrder);
        }
        private void GenerateGroupSmsMessages(Dictionary<ApplicationUser, IList<PhishingMessage>> campaignMessagesList)
        {
            var smsGroupTemplates = Campaign.TemplateUsage.Where(x => x.Type == Enums.TemplateType.Sms && x.SendType == Enums.TemplateSendType.Group);
            var groupSms = Math.Ceiling(Campaign.NumberOfSMSMessagesPerParticipant * 0.1);
            var groupSmsTemplatesRandomOrder = smsGroupTemplates.OrderBy(x => Randomizer.Next()).ToList();
            GenerateGroupMessages(campaignMessagesList, groupSms, groupSmsTemplatesRandomOrder);
        }
        private void GenerateGroupMessages(Dictionary<ApplicationUser, IList<PhishingMessage>> campaignMessagesList, double groupEmails, List<PhishingTemplate> groupEmailTemplatesRandomOrder)
        {
            int groupCount = 0;
            foreach (var template in groupEmailTemplatesRandomOrder)
            {
                if (groupCount >= groupEmails)
                    return;

                Logger.LogInformation(string.Format($"Generate group messages {DateTime.Now}"));
                if (!ValidateEmptyFields(template))
                    continue;

                var (templateStart, templateEnd) = InitializeTemplateDates(template);
                if (!ValidateTemplateTimeRange(template, templateStart, templateEnd))
                    continue;

                var timeToSend = RandomSendTime(templateStart, templateEnd, template.FromTimeOfDay, template.ToTimeOfDay);
                Logger.LogInformation(string.Format($"Add group message {template.Name} to every participant"));
                foreach (var participant in Campaign.Participants)
                {
                    if (campaignMessagesList[participant].Any(x => x.PhishingTemplateId == template.Id))
                        continue;

                    var phishingMessage = InitializeNewPhishingMessage(participant, template, timeToSend);

                    switch (template.Type)
                    {
                        case Enums.TemplateType.Email:
                            PrepareEmailContent(participant, template, phishingMessage);
                            break;
                        case Enums.TemplateType.Sms:
                            PrepareSmsContent(participant, template, phishingMessage);
                            break;
                        default:
                            PrepareDummyContent(participant, template, phishingMessage);
                            break;
                    }

                    campaignMessagesList[participant].Add(phishingMessage);
                }
                groupCount++;
            }
        }

        private PhishingMessage InitializeNewPhishingMessage(ApplicationUser participant, PhishingTemplate template, DateTimeOffset timeToSend)
        {
            return new PhishingMessage()
            {
                Id = Guid.NewGuid(),
                CampaignId = Campaign.Id,
                PhishingTemplateId = template.Id,
                PhishingTemplate = template,
                TimeToSend = timeToSend,
                UserId = participant.Id,
                User = participant,
                ClickDate = null,
                FetchImageDate = null
            };
        }

        private DateTimeOffset RandomSendTime(DateTimeOffset templateStart, DateTimeOffset templateEnd, TimeSpan? fromTimeOfDay, TimeSpan? toTimeOfDay)
        {
            var totalRuntime = templateEnd - templateStart;
            var randomRuntimeOffset = (long)(Randomizer.NextDouble() * totalRuntime.Ticks);
            DateTimeOffset dayToSend = templateStart.AddTicks(randomRuntimeOffset).Date;

            if (fromTimeOfDay.HasValue && toTimeOfDay.HasValue)
            {
                long addTimeTicks = 0;

                if (fromTimeOfDay < toTimeOfDay)
                {
                    var duration = toTimeOfDay.Value.Ticks - fromTimeOfDay.Value.Ticks;
                    addTimeTicks = fromTimeOfDay.Value.Ticks + (long)(Randomizer.NextDouble() * duration);
                }
                else
                {
                    long dayTicks = new TimeSpan(1, 0, 0, 0).Ticks;
                    var firstRange = toTimeOfDay.Value.Ticks;
                    var secondRange = dayTicks - fromTimeOfDay.Value.Ticks;
                    var fullRange = firstRange + secondRange;

                    addTimeTicks = (long)(Randomizer.NextDouble() * fullRange);

                    if (addTimeTicks > firstRange)
                        addTimeTicks = fromTimeOfDay.Value.Ticks + (addTimeTicks - firstRange);
                }

                dayToSend = dayToSend.AddTicks(addTimeTicks);
            }

            return dayToSend;
        }

        private bool ValidateTemplateTimeRange(PhishingTemplate template, DateTimeOffset templateStart, DateTimeOffset templateEnd)
        {
            if (Campaign.Start.Year == Campaign.End.Year || templateStart.Year != templateEnd.Year)
            {
                if (templateEnd < Campaign.Start || templateStart > Campaign.End)
                {
                    Logger.LogWarning(string.Format($"Template {template.Name} is outside of the campaing time - ignore this template."));
                    return false;
                }
            }
            else
            {
                // TODO Campaign goes into next year and template time is within a single year - need to check the template range in both years!
            }
            return true;
        }

        private (DateTimeOffset templateStart, DateTimeOffset templateEnd) InitializeTemplateDates(PhishingTemplate template)
        {
            var templateStart = new DateTimeOffset(new DateTime(Campaign.Start.Year, template.From.Value.Month, template.From.Value.Day));
            var templateEnd = new DateTimeOffset(new DateTime(Campaign.Start.Year, template.To.Value.Month, template.To.Value.Day));

            if (templateStart > templateEnd)
                templateEnd = templateEnd.AddYears(1);

            if (templateStart < Campaign.Start)
                templateStart = Campaign.Start;

            if (templateEnd > Campaign.End)
                templateEnd = Campaign.End;

            return (templateStart, templateEnd);
        }

        private bool ValidateEmptyFields(PhishingTemplate template)
        {
            if (template.From == null || template.To == null)
            {
                Logger.LogWarning(string.Format($"Template {template.Name} missing From or To values - ignore this template."));
                return false;
            }
            return true;
        }

        private async Task<Dictionary<Guid, TemplateTexts>> LoadTemplateLookup(IList<PhishingTemplate> campaignTemplates)
        {
            var templateDataStoragePath = Configuration.TemplateDataStoragePath();
            var textLookupById = new Dictionary<Guid, TemplateTexts>();
            foreach (var template in campaignTemplates.ToList())
            {
                TemplateTexts texts = new TemplateTexts();

                var htmlFilePath = $"{templateDataStoragePath}/{template.Id}/{template.Id}.html";
                if (File.Exists(htmlFilePath))
                {
                    using (var streamreader = new StreamReader(htmlFilePath))
                    {
                        texts.HtmlText = await streamreader.ReadToEndAsync();
                    }
                }

                var plainFilePath = $"{templateDataStoragePath}/{template.Id}.txt";
                if (File.Exists(plainFilePath))
                {
                    using (var streamreader = new StreamReader(plainFilePath))
                    {
                        texts.PlainText = await streamreader.ReadToEndAsync();
                    }
                }


                textLookupById.Add(template.Id, texts);
            }

            return textLookupById;
        }

        struct TemplateTexts
        {
            public string HtmlText { get; set; }
            public string PlainText { get; set; }
        }

        private void PrepareSmsContent(ApplicationUser participant, PhishingTemplate template, PhishingMessage message)
        {
            var plainBody = bodyByTemplateId[template.Id].PlainText;
            PreparePlainTextBody(participant, template, message, plainBody);
            PrepareSenderName(participant, template, message);
        }

        private void PrepareEmailContent(ApplicationUser participant, PhishingTemplate template, PhishingMessage message)
        {
            //var htmlBody = "<strong>from your parents: {FatherFirstName} &amp; {MotherFirstName}</strong>";
            var htmlBody = bodyByTemplateId[template.Id].HtmlText;
            var plainBody = bodyByTemplateId[template.Id].PlainText;

            PrepareHtmlBody(participant, template, message, htmlBody);
            PreparePlainTextBody(participant, template, message, plainBody);

            var subject = template.SubjectTemplate;
            PrepareSubject(participant, template, message, subject);

            PrepareSenderName(participant, template, message);
        }

        private void PrepareDummyContent(ApplicationUser participant, PhishingTemplate template, PhishingMessage message)
        {
            var plainBody = bodyByTemplateId[template.Id].PlainText;
            PreparePlainTextBody(participant, template, message, plainBody);
        }

        private void PrepareSubject(ApplicationUser participant, PhishingTemplate template, PhishingMessage message, string subject)
        {
            if (!String.IsNullOrEmpty(subject))
            {
                subject = ReplaceCampaignTokens(template, message, subject);
                subject = ReplaceUserTokens(participant, subject);
                message.Subject = subject;
            }
        }

        private void PrepareSenderName(ApplicationUser participant, PhishingTemplate template, PhishingMessage message)
        {
            if (!String.IsNullOrEmpty(template.SenderName))
            {
                var senderName = ReplaceCampaignTokens(template, message, template.SenderName);
                senderName = ReplaceUserTokens(participant, senderName);
                message.SenderName = senderName;
            }
            else
            {
                message.SenderName = String.Empty;
            }
        }

        private void PreparePlainTextBody(ApplicationUser participant, PhishingTemplate template, PhishingMessage message, string plainBody)
        {
            if (!String.IsNullOrEmpty(plainBody))
            {
                plainBody = ReplaceCampaignTokens(template, message, plainBody);
                plainBody = ReplaceUserTokens(participant, plainBody);
                message.TextBody = plainBody;
            }
        }

        private void PrepareHtmlBody(ApplicationUser participant, PhishingTemplate template, PhishingMessage message, string htmlBody)
        {
            if (!String.IsNullOrEmpty(htmlBody))
            {
                htmlBody = ReplaceCampaignTokens(template, message, htmlBody);
                htmlBody = ReplaceUserTokens(participant, htmlBody);
                var htmlDocument = ReplaceImgTagSources(htmlBody, message);
                message.HtmlBody = htmlDocument.DocumentNode.OuterHtml;
            }
        }

        private string ReplaceUserTokens(ApplicationUser participant, string text)
        {
            foreach (var token in UserMetadatas)
            {
                var participantPropertyValue = participant.GetPropertyValue(token, Localizer) ?? String.Empty;
                text = text?.Replace(ReplacementToken(token), participantPropertyValue);

                if (String.IsNullOrEmpty(participantPropertyValue))
                {
                    //TODO metadata not available for current participant. Empty replaced for now.
                    Logger.LogWarning(string.Format($"User {participant.UserName} has not entered the property {token} in her profile!"));
                }
            }
            return text;
        }

        public static string ReplacementToken(string token)
        {
            return $"{{{token}}}";
        }

        private string ReplaceCampaignTokens(PhishingTemplate template, PhishingMessage message, string text)
        {
            var domain = template.Origin ?? GetHostUrl();
            text = text.Replace("{Domain}", domain);
            text = text.Replace("{MessageId}", message.Id.ToString());
            text = text.Replace("{ClassTeacher}", Campaign.ClassTeacher);
            text = text.Replace("{School}", Campaign.School);
            text = text.Replace("{Class}", Campaign.Class);
            text = text.Replace("{InformaticsTeacher}", Campaign.InformaticsTeacher);
            text = text.Replace("{Director}", Campaign.Director);
            return text;
        }

        private string GetHostUrl()
        {
            var url = $"{HttpContextAccessor.HttpContext.Request.Scheme}://{HttpContextAccessor.HttpContext.Request.Host}";
            return url;
        }

        private HtmlDocument ReplaceImgTagSources(string htmlBody, PhishingMessage message)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlBody);
            var imgNodes = htmlDocument.DocumentNode.SelectNodes("//img");

            if (imgNodes != null && imgNodes.Count > 0)
            {

                foreach (var imgNode in imgNodes)
                {
                    var srcAttribute = imgNode.Attributes.FirstOrDefault(x => x.Name == "src");
                    if (srcAttribute == null) continue;

                    var hostname = message.PhishingTemplate.Origin ?? GetHostUrl();
                    var uri = new UriBuilder(hostname)
                    {
                        Path = "/Asset/GetAsset",
                        Query = $"messageId={message.Id}&asset={HttpUtility.UrlEncode(srcAttribute.Value)}"
                    };
                    var resultUrl = uri.ToString();
                    srcAttribute.Value = resultUrl;
                }
            }
            return htmlDocument;
        }
    }
}
