using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Triage.Api.Domain.Messages;
using Triage.Api.Domain.Messages.Aggregates;
using Triage.Business.Messages;

namespace Triage.UI.Mvc.Hubs
{
    public class LogHub : Hub, ILogHub
    {
        public void HourlyErrorUpdate(IEnumerable<ErrorMessagesBySource> currentHourErrors)
        {
            var logHub = GlobalHost.ConnectionManager.GetHubContext<LogHub>();
            logHub.Clients.All.hourlyErrorUpdate(currentHourErrors
                .Select(errorMessage => new
                {
                    Date = errorMessage.Date.ToShortDateString(),
                    errorMessage.Hour,
                    errorMessage.StackTrace,
                    errorMessage.Title,
                    errorMessage.Count
                }));
        }

        public void Notify()
        {
            var logHub = GlobalHost.ConnectionManager.GetHubContext<LogHub>();
            logHub.Clients.All.notify();
        }

        public void MessageUpdate(IList<Message> messages)
        {
            var logHub = GlobalHost.ConnectionManager.GetHubContext<LogHub>();
            logHub.Clients.All.messageUpdate(messages
                .Select(message => new
                {
                    Date = message.Date.ToString("g"),
                    Type = message.Type.ToString(),
                    message.Title,
                }));
        }
    }

}