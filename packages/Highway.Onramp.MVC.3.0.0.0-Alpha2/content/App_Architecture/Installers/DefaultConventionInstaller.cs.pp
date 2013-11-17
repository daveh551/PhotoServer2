// [[Highway.Onramp.MVC]]
using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Windsor;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using $rootnamespace$.App_Architecture.Services;

namespace $rootnamespace$.App_Architecture.Installers
{
    public class DefaultConventionInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().Pick()
                    .WithServiceDefaultInterfaces()
                    .Unless(type => typeof(IController).IsAssignableFrom(type))
                    .LifestylePerWebRequest()
                );
        }
    }
}
