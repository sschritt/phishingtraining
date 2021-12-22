using System;
using System.ComponentModel.DataAnnotations;

namespace PhishingTraining.Web.Entities.Base
{
    public abstract class KeyedEntity : IEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
