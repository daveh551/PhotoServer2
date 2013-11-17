// [[Highway.Onramp.MVC]]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace $rootnamespace$.App_Architecture.Services.Core
{
    public class WindsorFilterProvider : IFilterProvider
    {
        private readonly IEnumerable<IInjectableFilter> registeredFilters;

        public WindsorFilterProvider(IInjectableFilter[] registeredFilters)
        {
            this.registeredFilters = registeredFilters;
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return registeredFilters
                .Where(e => e.IsValid(controllerContext, actionDescriptor))
                .Select(e => new Filter(e,e.Scope,e.Order));
        }
    }
}
