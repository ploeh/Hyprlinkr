using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    /// A route tuple: a rouple - pardon the pun.
    /// </summary>
    public class Rouple
    {
        private readonly string routeName;
        private readonly IDictionary<string, object> routeValues;

        public Rouple(string routeName, IDictionary<string, object> routeValues)
        {
            if (routeName == null)
                throw new ArgumentNullException("routeName");
            if (routeValues == null)
                throw new ArgumentNullException("routeValues");
                        
            this.routeName = routeName;
            this.routeValues = routeValues;
        }

        public string RouteName
        {
            get { return this.routeName; }
        }

        public IDictionary<string, object> RouteValues
        {
            get { return this.routeValues; }
        }
    }
}
