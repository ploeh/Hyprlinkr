using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ploeh.Hyprlinkr
{
    public class DefaultRouteValuesQuery : IRouteValuesQuery
    {
        public IDictionary<string, object> GetRouteValues(
            MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression == null)
                throw new ArgumentNullException("methodCallExpression");

            var parameters =
                methodCallExpression.Method.GetParameters().SingleOrDefault();
            if (parameters == null)
                return new Dictionary<string, object>();

            return this.GetParameterValues(
                methodCallExpression,
                methodCallExpression.Method.GetParameters().Single());
        }

        public virtual IDictionary<string, object> GetParameterValues(
            MethodCallExpression methodCallExpression,
            ParameterInfo parameterInfo)
        {
            throw new ArgumentNullException();
        }
    }
}
