using System;

namespace PhishingTraining.Web.Services.Interfaces
{
    public interface IApplicationSender
    {
        void SendPhishingMessage(Guid phishingMessageId);
        void SendTestPhishingMessage(Guid phishingMessageId, string receiver);
    }
}