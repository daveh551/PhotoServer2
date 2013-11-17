// [[Highway.Onramp.MVC]]
using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Logging;
using System.Collections.Generic;

namespace PhotoServer2.App_Architecture.BaseTypes
{
    public class BaseController : Controller
    {
        public ILogger Logger { get; set; }

        public BaseController()
        {
            Logger = NullLogger.Instance;
        }
    }
}