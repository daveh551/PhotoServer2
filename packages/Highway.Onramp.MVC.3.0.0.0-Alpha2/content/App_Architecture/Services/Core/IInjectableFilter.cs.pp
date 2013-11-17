// [[Highway.Onramp.MVC]]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Logging;
using Castle.Windsor;

namespace $rootnamespace$.App_Architecture.Services.Core
{
    public interface IInjectableFilter
    {
        bool IsValid(ControllerContext context, ActionDescriptor descriptor);
        FilterScope Scope { get; }
        int? Order { get; }
    }
}
