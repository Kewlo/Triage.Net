using System;
using System.Linq;
using System.Web.Http;
using SimpleInjector.Integration.WebApi;
using Triage.DomainController;
using Triage.Persistence.Context;
using Triage.Web.Api.Controllers;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Triage.UI.Mvc.App_Start.SimpleInjectorInitializer), "Initialize")]

namespace Triage.UI.Mvc.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;

    using SimpleInjector;
    using SimpleInjector.Extensions;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;
    
    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            // Did you know the container can diagnose your configuration? 
            // Go to: https://simpleinjector.org/diagnostics
            var container = new Container();
            
            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            
            container.Verify();

            var resolver = new SimpleInjectorDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
     
        private static void InitializeContainer(Container container)
        {
            container.AutoMap(
                Assembly.GetExecutingAssembly(), 
                Assembly.GetAssembly(typeof(ITriageDbContextFactory)),
                Assembly.GetAssembly(typeof(IEventLogController)),
                Assembly.GetAssembly(typeof(ITriageDbContextFactory)),
                Assembly.GetAssembly(typeof(LogController))
                );

            container.Register<LogController>();
            container.Register<ITriageDbContextFactory, TriageDbContextFactory>(Lifestyle.Singleton);
            // For instance:
            // container.Register<IUserRepository, SqlUserRepository>();
        }
    }

    public static class SimpleInjectorExtensions
    {
        public static void AutoMap(this Container container, params Assembly[] assemblies)
        {
            container.ResolveUnregisteredType += (s, e) =>
            {
                if (e.UnregisteredServiceType.IsInterface && !e.Handled)
                {
                    Type[] concreteTypes = (
                        from assembly in assemblies
                        from type in assembly.GetTypes()
                        where !type.IsAbstract && !type.IsGenericType
                        where e.UnregisteredServiceType.IsAssignableFrom(type)
                        select type)
                        .ToArray();

                    if (concreteTypes.Length == 1)
                    {
                        e.Register(Lifestyle.Transient.CreateRegistration(concreteTypes[0],
                            container));
                    }
                }
            };
        }
    }
}