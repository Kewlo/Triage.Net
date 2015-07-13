using System.Collections.Generic;
using Triage.Api.Domain.Messages;

namespace Triage.Business.Messages
{
    public interface ILogHub
    {
        void SendNewMessages(IEnumerable<Message> messages);
    }
}