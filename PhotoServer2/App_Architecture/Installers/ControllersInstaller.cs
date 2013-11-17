// [[Highway.Onramp.MVC]]
using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Windsor;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using PhotoServer2.App_Architecture.Services;

namespace PhotoServer2.App_Architecture.Installers
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .Where(type => typeof(IController).IsAssignableFrom(type))
                    .WithServiceSelf()
                    .LifestyleTransient()
                );
        }
    }
}