using System.ComponentModel.DataAnnotations;

namespace PhishingTraining.Web.Entities.Base
{
    public abstract class NamedEntity : KeyedEntity
    {
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength]
        public string Description { get; set; }
    }
}