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
        private readonly string routeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRouteDispatcher" /> class.
        /// </summary>
        public DefaultRouteDispatcher()
            : this("API Default")
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DefaultRouteDispatcher" /> class with the supplied route name.
        /// </summary>
        /// <param name="routeName">
        /// The route name which will be used by the <see cref="Dispatch" /> method as the
        /// <see cref="Rouple.RouteName" /> value.
        /// </param>
        /// <remarks>
        /// <para>
        /// After initialization, the <paramref name="routeName" /> value is available through the
        /// <see cref="RouteName" /> property.
        /// </para>
        /// </remarks>
        public DefaultRouteDispatcher(string routeName)
        {
            if (routeName == null)
                throw new ArgumentNullException("routeName");

            this.routeName = routeName;
        }

        /// <summary>
        /// Provides dispatch information based on an Action Method.
        /// </summary>
        /// <param name="method">The Action Method.</param>
        /// <param name="routeValues">Route values.</param>
        /// <returns>
        /// An object containing the route name, as well as the route values.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The returned <see cref="Rouple.RouteName" /> will be the value of the
        /// <see cref="RouteName" /> property.
        /// </para>
        /// <para>
        /// The returned <see cref="Rouple.RouteValues" /> will be all entries of the
        /// <paramref name="routeValues" />, plus a value for an additional "controller" key,
        /// derived from <paramref name="method" />.
        /// </para>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "This method should produce URIs with lower-case letters, so ultimately, it would have to invoke some sort of ToLower method.")]
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

            return new Rouple(this.routeName, newRouteValues);
        }

        /// <summary>
        /// Gets the route name.
        /// </summary>
        /// <seealso cref="DefaultRouteDispatcher(string)" />
        public string RouteName
        {
            get { return this.routeName; }
        }
    }
}
