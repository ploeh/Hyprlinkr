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

            return methodCallExpression.Method.GetParameters()
                .Select(p => this.GetParameterValues(methodCallExpression, p))
                .SelectMany(d => d)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public virtual IDictionary<string, object> GetParameterValues(
            MethodCallExpression methodCallExpression,
            ParameterInfo parameterInfo)
        {
            if (methodCallExpression == null)
                throw new ArgumentNullException("methodCallExpression");
            if (parameterInfo == null)
                throw new ArgumentNullException("parameterInfo");

            var arg = methodCallExpression.Arguments[parameterInfo.Position];
            var lambda = Expression.Lambda(arg);
            var value = lambda.Compile().DynamicInvoke().ToString();
            return new Dictionary<string, object>
            {
                { parameterInfo.Name, value }
            };
        }
    }
}
