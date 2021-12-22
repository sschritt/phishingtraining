using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhishingTraining.Web.Data;

namespace PhishingTraining.Web.Services
{
    public abstract class ServiceBase
    {
        protected ApplicationDbContext DbContext { get; }
        protected IConfiguration Configuration { get; }
        protected ServiceBase(IServiceProvider serviceProvider)
        {
            DbContext = serviceProvider.GetService<ApplicationDbContext>();
            Configuration = serviceProvider.GetService<IConfiguration>();
        }
    }
}