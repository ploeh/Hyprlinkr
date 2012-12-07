using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ploeh.Hyprlinkr
{
    public interface IRouteValuesQuery
    {
        IDictionary<string, object> GetRouteValues(
            MethodCallExpression methodCallExp);
    }
}
