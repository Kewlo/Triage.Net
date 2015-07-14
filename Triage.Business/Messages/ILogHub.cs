using System.Collections.Generic;
using Triage.Api.Domain.Messages;
using Triage.Api.Domain.Messages.Aggregates;

namespace Triage.Business.Messages
{
    public interface ILogHub
    {
        void HourlyErrorUpdate(IEnumerable<ErrorMessagesBySource> currentHourErrors);
        void Notify();
        void MessageUpdate(IList<Message> messages);
    }
}