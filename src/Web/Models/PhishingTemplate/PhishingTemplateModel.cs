using Microsoft.AspNetCore.Mvc;
using PhishingTraining.Web.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PhishingTraining.Web.Models.PhishingTemplate
{
    public class PhishingTemplateModel
    {
        public Guid? Id { get; set; }
        [Display(Name = nameof(Name))]
        public string Name { get; set; }
        [Display(Name = nameof(Origin))]
        public string Origin { get; set; }
        [Display(Name = nameof(Description))]
        public string Description { get; set; }
        [Display(Name = nameof(TemplateType))]
        public TemplateType Type { get; set; }

        [Display(Name = nameof(From))]
        public DateTimeOffset From { get; set; }
        [Display(Name = nameof(To))]
        public DateTimeOffset To { get; set; }
        //[Display(Name = nameof(MinTimeBetweenMessages))]
        //public Double? MinTimeBetweenMessagesInSeconds { 
        //    get => MinTimeBetweenMessages?.TotalSeconds; 
        //    set { MinTimeBetweenMessages = (value.HasValue) ? TimeSpan.FromSeconds(value.Value) : null; } }
        //[UIHint("TimespanDistance")]
        public int MinSecondsBetweenMessages { get; set; }
        [Display(Name = nameof(FromTimeOfDay))]
        public TimeSpan? FromTimeOfDay { get ; set; }
        [Display(Name = nameof(ToTimeOfDay))]
        public TimeSpan? ToTimeOfDay { get; set; }
        [Display(Name = nameof(SendType))]
        public TemplateSendType SendType { get; set; }
        [Display(Name = nameof(Difficulty))]
        public DifficultyType Difficulty { get; set; }
        [Display(Name = nameof(SenderName))]
        public string SenderName { get; set; }
        [Display(Name = nameof(SenderAddress))]
        public string SenderAddress { get; set; }
        [Display(Name = nameof(SenderKnown))]
        public bool SenderKnown { get; set; }
        [Display(Name = nameof(Incentive))]
        public IncentiveType Incentive { get; set; }
        [Display(Name = nameof(TemplateFile))]
        public IFormFile TemplateFile { get; set; }
        [Display(Name = nameof(PlainTemplateFile))]
        public IFormFile PlainTemplateFile { get; set; }
        [Display(Name = nameof(SubjectTemplate))]
        public string SubjectTemplate { get; set; }
        public string EducationalInformation { get; set; }        

        public Guid? CopyTemplateFilesFromTemplateId { get; set; }
    }
}
