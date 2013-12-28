using System.Web.Http;
using System.Web.Http.Controllers;
using MediaTypeFormatters;

namespace RacePhotosTestSupport
{
    public class FakeControllerContext : HttpControllerContext
    {
        public FakeControllerContext()
        {
            Configuration = new HttpConfiguration();
            Configuration.Formatters.Add(new JpegMediaTypeFormatter());
            // Setup configuration with routes, etc. as per application
            PhotoServer2.WebApiConfig.Register(Configuration);


        }



    }



}
