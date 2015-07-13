using System.Web.Http;

namespace Triage.UI.Mvc
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            //config.MapHttpAttributeRoutes();
            //config.Services.Replace(typeof(IAssembliesResolver), new TriageApiResolver());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new
                {
                    action = "Index",
                    id = RouteParameter.Optional
                }
            );
        }

    }
}
