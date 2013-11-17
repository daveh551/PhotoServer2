// [[Highway.Onramp.MVC]]
using System;
using System.Linq;
using Castle.Windsor;
using Castle.Facilities.Logging;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

namespace $rootnamespace$.App_Architecture.Installers
{
    public class LoggingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            
            container.AddFacility<LoggingFacility>(m => m.UseNLog().WithConfig("NLog.config"));
        }
    }
}