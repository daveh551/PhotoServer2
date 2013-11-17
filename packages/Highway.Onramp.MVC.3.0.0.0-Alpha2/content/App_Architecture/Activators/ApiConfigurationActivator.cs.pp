// [[Highway.Onramp.MVC]]
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Http.Dispatcher;
using System.Web.Http;
using $rootnamespace$.App_Architecture.Activators;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof(ApiConfigurationActivator),
    "PostStartup")]
namespace $rootnamespace$.App_Architecture.Activators
{
    public static class ApiConfigurationActivator
    {
        public static void PostStartup()
        {
#pragma warning disable 618
            var activator = IoC.Container.Resolve<IHttpControllerActivator>();
#pragma warning restore 618
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), activator);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings =
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
        }
    }
}
