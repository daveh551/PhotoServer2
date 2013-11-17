// [[Highway.Onramp.MVC]]
using System;
using System.Linq;
using Castle.Windsor;
using Castle.MicroKernel;
using Castle.Windsor.Installer;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Components.DictionaryAdapter;
using System.Configuration;
using $rootnamespace$.App_Architecture.Activators;
using $rootnamespace$.App_Architecture.Services.Core;
using System.Web;
using System.Collections;

[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof(WindsorActivator), 
    "Startup")]
namespace $rootnamespace$.App_Architecture.Activators
{
    public static class WindsorActivator
    {
        public static void Startup()
        {
#pragma warning disable 618
            // Create the container
            IoC.Container = new WindsorContainer();

            // Add the Array Resolver, so we can take dependencies on T[]
            // while only registering T.
            IoC.Container.Kernel.Resolver.AddSubResolver(new ArrayResolver(IoC.Container.Kernel));

            // Register the kernel and container, in case an installer needs it.
            IoC.Container.Register(
                Component.For<IKernel>().Instance(IoC.Container.Kernel),
                Component.For<IWindsorContainer>().Instance(IoC.Container)
                );

            // Our configuration magic, register all interfaces ending in Config from
            // this assembly, and create implementations using DictionaryAdapter
            // from the AppSettings in our app.config.
            var daf = new DictionaryAdapterFactory();
            IoC.Container.Register(
                Types
                    .FromThisAssembly()
                    .Where(type => type.IsInterface && type.Name.EndsWith("Config"))
                    .Configure(
                        reg => reg.UsingFactoryMethod(
                            (k, m, c) => daf.GetAdapter(m.Implementation, ConfigurationManager.AppSettings)
                            )
                    ));

            // Our session magic, register all interfaces ending in Session from
            // this assembly, and create implementations using DictionaryAdapter
            // from the current HttpSession
            IoC.Container.Register(
                Types
                    .FromThisAssembly()
                    .Where(type => type.IsInterface && type.Name.EndsWith("Session"))
                    .Configure(
                        reg => reg.UsingFactoryMethod(
                            (k, m, c) => daf.GetAdapter(m.Implementation, new SessionDictionary(HttpContext.Current.Session) as IDictionary)
                            )
                    ).LifestylePerWebRequest());


            // Search for an use all installers in this application.
            IoC.Container.Install(FromAssembly.This());

#pragma warning restore 618
        }
    }
}