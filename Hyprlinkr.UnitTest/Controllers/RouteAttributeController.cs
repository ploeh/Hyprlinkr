using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    [Route("/controllerCustomRoute", Name = ControllerRouteName)]
    public class RouteAttributeController : ApiController
    {
        public const string ControllerRouteName = "ControllerCustomRoute";

        public const string ActionRouteName = "CustomRoute";

        [Route("/customRoute", Name = ActionRouteName)]
        public object GetDefault()
        {
            return new object();
        }
        
        public object GetDefaultWithoutActionRouteName()
        {
            return new object();
        }
    }
}
