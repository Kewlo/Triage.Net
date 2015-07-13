using Microsoft.Owin;
using Owin;
using Triage.UI.Mvc;

[assembly: OwinStartup(typeof(SignalRConfig), "Register")]
namespace Triage.UI.Mvc
{
    public class SignalRConfig
    {
        public void Register(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}