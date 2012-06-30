using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ploeh.Hyprlinkr
{
    public class DefaultRouteDispatcher : IRouteDispatcher
    {
        public Rouple Dispatch(
            MethodInfo method,
            IDictionary<string, object> routeValues)
        {
            var newRouteValues = new Dictionary<string, object>(routeValues);

            var controllerName = method
                .ReflectedType
                .Name
                .ToLowerInvariant()
                .Replace("controller", "");
            newRouteValues["controller"] = controllerName;

            return new Rouple("API Default", newRouteValues);
        }
    }
}
