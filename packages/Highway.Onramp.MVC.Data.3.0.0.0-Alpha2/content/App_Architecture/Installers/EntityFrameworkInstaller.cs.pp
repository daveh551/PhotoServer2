// [[Highway.Onramp.MVC.Data]]
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using System.Data.Entity;
using $rootnamespace$.App_Architecture.Configs;
using $rootnamespace$.App_Architecture.Services.Data;

namespace $rootnamespace$.App_Architecture.Installers
{
    public class EntityFrameworkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var config = container.Resolve<IDatabaseInitializerConfig>();
            switch (config.Initializer)
            {
                case Configs.InitializerTypes.DropCreateDatabaseAlways:
                    container.Register(Component.For<IDatabaseInitializer<HighwayDataContext>>()
                        .ImplementedBy<DropCreateDatabaseAlways<HighwayDataContext>>().LifestyleSingleton());
                    break;
                case Configs.InitializerTypes.DropCreateDatabaseIfModelChanges:
                    container.Register(Component.For<IDatabaseInitializer<HighwayDataContext>>()
                        .ImplementedBy<DropCreateDatabaseIfModelChanges<HighwayDataContext>>().LifestyleSingleton());
                    break;
                case Configs.InitializerTypes.CreateDatabaseIfNotExists:
                    container.Register(Component.For<IDatabaseInitializer<HighwayDataContext>>()
                        .ImplementedBy<CreateDatabaseIfNotExists<HighwayDataContext>>().LifestyleSingleton());
                    break;
                case Configs.InitializerTypes.NullDatabaseInitializer:
                    container.Register(Component.For<IDatabaseInitializer<HighwayDataContext>>()
                        .ImplementedBy<NullDatabaseInitializer<HighwayDataContext>>().LifestyleSingleton());
                    break;
                default:
                    throw new NotImplementedException("Unknown Enumeration Value");
            }
        }
    }
}
