using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using MediaTypeFormatters;

namespace PhotoServer_Tests
{
    public class FakeControllerContext : HttpControllerContext
    {
        public FakeControllerContext(ApiController controller, HttpRequestMessage request)
        {
            // Set this before anything else, because the Configuration property getter
            // pulls it from the RequestContext;
            RequestContext = new HttpRequestContext();
            Configuration = new HttpConfiguration();
            RequestContext.Configuration = Configuration;
            // Setup configuration with routes, etc. as per application
            PhotoServer2.WebApiConfig.Register(Configuration);
	        RequestContext.Principal = new GenericPrincipal(new GenericIdentity("FinishLineAdmin"), new string[0]);
	        var routeValue = new HttpRouteValueDictionary();
	        routeValue.Add("controller", "Photos");
	        var routeData = new HttpRouteData(Configuration.Routes["DefaultApi"], routeValue);
            Configuration.EnsureInitialized();

            request.SetConfiguration(Configuration);
            request.SetRouteData(routeData);

            Request = request;
            Controller = controller;
            ControllerDescriptor = new HttpControllerDescriptor(Configuration,"Photos", controller.GetType());


        }



    }



}
