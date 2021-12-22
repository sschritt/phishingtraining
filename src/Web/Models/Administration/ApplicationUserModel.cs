using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Models.Administration
{
    public class ApplicationUserModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Administrator")]
        public bool IsAdministrator { get; set; }
        [Display(Name = "Manager")]
        public bool IsManager { get; set; }
    }
}