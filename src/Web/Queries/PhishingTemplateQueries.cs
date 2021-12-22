using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhishingTraining.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Queries
{
    public static class PhishingTemplateQueries
    {
        public static IIncludableQueryable<PhishingTemplate, IList<UserMetadata>> IncludeParameters(this IQueryable<PhishingTemplate> query) =>
          query.Include(template => template.Parameters);
    }
}
