using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class RouteAttributeController : ApiController
    {
        public const string RouteName = "CustomRoute";

        [Route("/customRoute", Name = RouteName)]
        public object GetDefault()
        {
            return new object();
        }
    }
}
