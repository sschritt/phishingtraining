using System.Collections.Generic;
using PhishingTraining.Web.Entities.Base;

namespace PhishingTraining.Web.Entities
{
    public class UserMetadata : NamedEntity
    {
        public string TemplatePlaceholder { get; set; }

        public IList<PhishingTemplate> Templates { get; set; }
    }

    public static class UserMetadataExtentions
    {
        public static string ReplacementToken(this UserMetadata metadata)
        {
            return $"{{{metadata.TemplatePlaceholder}}}";
        }
    }
}