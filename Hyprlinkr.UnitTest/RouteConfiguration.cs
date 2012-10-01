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
            request.RequestUri = new Uri(request.RequestUri, "api/qux");
            request.AddRoute(
                name: "API Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

        public static void AddRoute(this HttpRequestMessage request,
            string name,
            string routeTemplate,
            object defaults)
        {
            var config = request.GetConfiguration();

            config.AddRoute(name, routeTemplate, defaults);

            request.Properties[HttpPropertyKeys.HttpRouteDataKey] =
                config.Routes.GetRouteData(request);
        }

        public static void AddDefaultRoute(this HttpConfiguration configuration)
        {
            configuration.AddRoute("Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }

        public static void AddRoute(this HttpConfiguration configuration, 
            string name, 
            string routeTemplate, 
            object defaults)
        {
            configuration.Routes.MapHttpRoute(name, routeTemplate, defaults);
        }
    }
}
