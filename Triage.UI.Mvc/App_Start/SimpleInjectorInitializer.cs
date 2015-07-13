using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SimpleInjector.Integration.WebApi;
using Triage.Business;
using Triage.Persistence.Context;
using Triage.Web.Api.Controllers;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Triage.UI.Mvc.App_Start.SimpleInjectorInitializer), "Initialize")]

namespace Triage.UI.Mvc.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;

    using SimpleInjector;
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
            var assemblies = GetInjectionAssemblies().ToList();

            container.AutoMapDefaults(assemblies);
            container.RegisterAllConcreteImplementations<IDbIndex>(assemblies);

            container.Register<LogController>();
            container.Register<ITriageDbContextFactory, TriageDbContextFactory>(Lifestyle.Singleton);
        }

        private static IEnumerable<Assembly> GetInjectionAssemblies()
        {
            yield return Assembly.GetExecutingAssembly();
            yield return Assembly.GetAssembly(typeof (ITriageDbContextFactory));
            yield return Assembly.GetAssembly(typeof (IEventLogController));
            yield return Assembly.GetAssembly(typeof (ITriageDbContextFactory));
            yield return Assembly.GetAssembly(typeof (LogController));
        }
    }

    public static class SimpleInjectorExtensions
    {
        public static void RegisterAllConcreteImplementations<T>(this Container container, IEnumerable<Assembly> assemblies)
        {
            var baseType = typeof (T);

            var concreteTypes = (
                from assembly in assemblies
                from type in assembly.GetTypes()
                where !type.IsAbstract && !type.IsGenericType
                where baseType.IsAssignableFrom(type)
                select type).Distinct();

            container.RegisterAll<T>(concreteTypes);
        }

        public static void AutoMapDefaults(this Container container, IEnumerable<Assembly> assemblies)
        {
            container.ResolveUnregisteredType += (s, e) =>
            {
                if (e.UnregisteredServiceType.IsInterface && !e.Handled)
                {
                    var concreteTypes = (
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