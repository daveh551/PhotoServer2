using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using PhotoServer.Storage;

namespace PhotoServer2.App_Architecture.Installers
{
    public class FileStorageProviderInstaller : IWindsorInstaller
    {
        private const string PhotoPath = "PhotosPhysicalDirectory";
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            if (PhotosPhysicalPathExists())
            {
                container.Register(
                    Component.For<IStorageProvider>()
                        .ImplementedBy<FileStorageProvider>()
                        .DynamicParameters(
                            (kernel, parameters) => parameters["root"] = ConfigurationManager.AppSettings[PhotoPath])
                        .LifestyleSingleton());
            }
            else
            {
                container.Register(
                    Component.For<IStorageProvider>()
                    .ImplementedBy<AzureStorageProvider>()
                    .DynamicParameters(
                        (kernel, parameters) =>
                        {
                            parameters["azureConnectionString"] =
                                ConfigurationManager.ConnectionStrings["AzureStorageConnection"];
                            parameters["container"] = "images";
                        })
                        .LifestyleSingleton());
            }
        }

        private static bool PhotosPhysicalPathExists()
        {
            var photosPhysicalPath = ConfigurationManager.AppSettings[PhotoPath];
			Trace.TraceInformation("Got PhotosPhysicalPath Configuration = {0} ", photosPhysicalPath);

            if (string.IsNullOrEmpty(photosPhysicalPath)) return false;
	        return (photosPhysicalPath.Substring(1, 2) == @":\");
        
        }
    }
}