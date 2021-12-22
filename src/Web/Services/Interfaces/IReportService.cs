using System;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateCsvReportForCampaign(Guid id);
    }
}