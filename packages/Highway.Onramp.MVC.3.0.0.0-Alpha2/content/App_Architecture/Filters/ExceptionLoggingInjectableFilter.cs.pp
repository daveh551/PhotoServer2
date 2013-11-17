// [[Highway.Onramp.MVC]]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Logging;
using Castle.Windsor;
using $rootnamespace$.App_Architecture.Services;
using $rootnamespace$.App_Architecture.Services.Core;

namespace $rootnamespace$.App_Architecture.Filters
{
    public class ExceptionLoggingInjectableFilter : IExceptionFilter, IInjectableFilter
    {
        public ILogger Logger { get; set; }

        public ExceptionLoggingInjectableFilter()
        {
            Logger = NullLogger.Instance;
        }

        public void OnException(ExceptionContext filterContext)
        {
            Logger.Error(filterContext.HttpContext.Request.Url.ToString(), filterContext.Exception);
        }

        public bool IsValid(ControllerContext context, ActionDescriptor descriptor)
        {
            return true;
        }

        public FilterScope Scope
        {
            get { return FilterScope.Last; }
        }

        public int? Order
        {
            get { return int.MinValue; }
        }
    }
}