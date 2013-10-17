using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace Ploeh.Hyprlinkr.UnitTest45
{
    public static class RouteConfiguration
    {
        public static void AddDefaultRoute(this HttpRequestMessage request)
        {
            request.RequestUri = new Uri(request.RequestUri, "api/qux");
            request.AddRoute(
                name: "DefaultApi",
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

        public static IHttpRoute AddRoute(this HttpConfiguration configuration,
            string name,
            string routeTemplate,
            object defaults)
        {
            return configuration.Routes.MapHttpRoute(name, routeTemplate, defaults);
        }
    }
}
