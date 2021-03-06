// [[Highway.Onramp.MVC.Data]]
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using PhotoServer2.App_Architecture.Activators;
using PhotoServer2.App_Architecture.Services.Data;

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof(DatabaseInitializerActivator), 
    "PostStartup")]
namespace PhotoServer2.App_Architecture.Activators
{
    public static class DatabaseInitializerActivator
    {
        public static void PostStartup()
        {
#pragma warning disable 618
            var initializer = IoC.Container.Resolve<IDatabaseInitializer<HighwayDataContext>>();
#pragma warning restore 618
            Database.SetInitializer(initializer);
        }
    }
}