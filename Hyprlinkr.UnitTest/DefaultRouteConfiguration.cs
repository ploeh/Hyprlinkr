using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public static class DefaultRouteConfiguration
    {
        public static void AddDefaultRoute(this HttpRequestMessage request)
        {
            var config = request.GetConfiguration();

            config.Routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "Home", id = RouteParameter.Optional });

            request.Properties[HttpPropertyKeys.HttpRouteDataKey] =
                config.Routes.GetRouteData(request);
        }
    }
}
