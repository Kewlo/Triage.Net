using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Triage.Api.Domain.Messages;
using Triage.Business.Messages;

namespace Triage.UI.Mvc.Hubs
{
    public class LogHub : Hub, ILogHub
    {
        public void SendNewMessages(IEnumerable<Message> messages)
        {
            var logHub = GlobalHost.ConnectionManager.GetHubContext<LogHub>();
            logHub.Clients.All.messageUpdate(messages);
        }

    }

}