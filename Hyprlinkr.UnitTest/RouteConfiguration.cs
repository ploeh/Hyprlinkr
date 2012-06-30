using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public static class RouteConfiguration
    {
        public static void AddDefaultRoute(this HttpRequestMessage request)
        {
            request.RequestUri = new Uri(request.RequestUri, "api");
            request.AddRoute(
                name: "API Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "Home", id = RouteParameter.Optional });
        }

        public static void AddRoute(this HttpRequestMessage request,
            string name,
            string routeTemplate,
            object defaults)
        {
            var config = request.GetConfiguration();

            config.Routes.MapHttpRoute(name, routeTemplate, defaults);

            request.Properties[HttpPropertyKeys.HttpRouteDataKey] =
                config.Routes.GetRouteData(request);
        }
    }
}
