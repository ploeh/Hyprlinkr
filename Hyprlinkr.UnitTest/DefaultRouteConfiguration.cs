using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public static class DefaultRouteConfiguration
    {
        public static void AddDefaultRoute(this HttpRequestMessage request)
        {
            request.GetConfiguration().Routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
