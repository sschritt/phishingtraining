using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Services.Interfaces
{
    public interface IMessageGenerator
    {
        Task GenerateMessages(Guid id);
    }
}
