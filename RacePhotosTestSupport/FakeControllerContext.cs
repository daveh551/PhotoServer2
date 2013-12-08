using System.Web.Http;
using System.Web.Http.Controllers;

namespace RacePhotosTestSupport
{
    public class FakeControllerContext : HttpControllerContext
    {
        public FakeControllerContext()
        {
            Configuration = new HttpConfiguration();
            // Setup configuration with routes, etc. as per application
            PhotoServer2.WebApiConfig.Register(Configuration);


        }



    }



}
