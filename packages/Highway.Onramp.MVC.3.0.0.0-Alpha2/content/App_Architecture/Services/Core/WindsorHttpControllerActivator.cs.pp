// [[Highway.Onramp.MVC]]
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http.Dispatcher;
using System.Web.Http.Controllers;
using System.Net.Http;
using Castle.Windsor;

namespace $rootnamespace$.App_Architecture.Services
{
    // Developed by Mark Seemann at http://blog.ploeh.dk/2012/10/03/DependencyInjectioninASP.NETWebAPIwithCastleWindsor/
    public class WindsorHttpControllerActivator : IHttpControllerActivator
    {
        private readonly IWindsorContainer container;

        public WindsorHttpControllerActivator(IWindsorContainer container)
        {
            this.container = container;
        }
        public IHttpController Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            var controller =
                (IHttpController)this.container.Resolve(controllerType);

            request.RegisterForDispose(
                new Release(
                    () => this.container.Release(controller)));

            return controller;
        }

        private class Release : IDisposable
        {
            private readonly Action release;

            public Release(Action release)
            {
                this.release = release;
            }

            public void Dispose()
            {
                this.release();
            }
        }
    }
}
