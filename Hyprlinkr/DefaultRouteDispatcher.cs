using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    /// The default Strategy for dispatching Action Methods to a route name, by
    /// always dispatching to a single, named route.
    /// </summary>
    /// <seealso cref="IRouteDispatcher" />
    public class DefaultRouteDispatcher : IRouteDispatcher
    {
        public Rouple Dispatch(
            MethodInfo method,
            IDictionary<string, object> routeValues)
        {
            if (method == null)
                throw new ArgumentNullException("method");

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
