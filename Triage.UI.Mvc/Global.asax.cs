using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;

namespace Triage.UI.Mvc
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }
}