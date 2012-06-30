using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ploeh.Hyprlinkr
{
    public interface IRouteDispatcher
    {
        Rouple Dispatch(MethodInfo method, IDictionary<string, object> routeValues);
    }
}
