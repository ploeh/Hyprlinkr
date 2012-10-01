using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using System.Web.Routing;

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

        public static IHttpRoute AddDefaultRoute(this HttpConfiguration configuration)
        {
            return configuration.AddRoute("Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }

        public static IHttpRoute AddRoute(this HttpConfiguration configuration, 
            string name, 
            string routeTemplate, 
            object defaults)
        {
            return configuration.Routes.MapHttpRoute(name, routeTemplate, defaults);
        }

        public static void AddPermanentRedirectRoute(this HttpConfiguration configuration, string name, IHttpRoute source, IHttpRoute target)
        {
            configuration.Routes.MapHttpRoute(name, source.RouteTemplate, source.Defaults, source.Constraints, new RedirectRouteHandler(target, true));
        }
    }
}
