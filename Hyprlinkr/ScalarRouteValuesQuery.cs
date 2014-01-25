using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    /// Represents a query which projects an expression's parameter values into
    /// a dictionary by converting each parameter value to a string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation of <see cref="IRouteValuesQuery" /> assumes that
    /// each method argument is a scalar (i.e. primitive) value which can be
    /// converted to a string.
    /// </para>
    /// <para>
    /// To support more sophisticated projection of route values the
    /// <see cref="GetParameterValues" /> method can be overridden in a derived
    /// class.
    /// </para>
    /// </remarks>
    public class ScalarRouteValuesQuery : IRouteValuesQuery
    {
        /// <summary>Gets the route values.</summary>
        /// <param name="methodCallExpression">
        /// A method call expression.
        /// </param>
        /// <returns>
        /// A dictionary of the method's arguments converted into strings.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// methodCallExpression is null
        /// </exception>
        /// <remarks>
        /// <para>
        /// A <see cref="MethodCallExpression" /> represents a call to a
        /// method. If the method takes arguments, the expression contains
        /// these arguments. The GetRouteValues method turns the argument
        /// values into strings and builds a dictionary of them, assuming that
        /// each value is a scalar (i.e. primitive) value which can be directly
        /// converted into a string.
        /// </para>
        /// <para>
        /// Each parameter is resolved into a dictionary by invoking the
        /// virtual <see cref="GetParameterValues" /> method, so a derived
        /// class can override this Template Method to support more
        /// sophisticated serialization scenarios.
        /// </para>
        /// </remarks>
        /// <seealso cref="GetParameterValues" />
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

        /// <summary>Gets the parameter value for a method argument.</summary>
        /// <param name="methodCallExpression">
        /// A method call expression.
        /// </param>
        /// <param name="parameterInfo">
        /// The parameter info for which the value should be returned.
        /// </param>
        /// <returns>
        /// A dictionary with the single (scalar) value of the method argument
        /// identified by <paramref name="parameterInfo" />
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// methodCallExpression is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// parameterInfo is null
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method is called by <see cref="GetRouteValues" /> to serialize
        /// method arguments into route values. It assumes that the method
        /// argument is a scalar (i.e. primitive) value which can be converted
        /// into a single string. The returned dictionary contains this scalar
        /// string keyed by the name of the parameter.
        /// </para>
        /// <para>
        /// This is a Template Method, so derived classes can override it to
        /// support more sophisticated serialization scenarios.
        /// </para>
        /// </remarks>
        /// <seealso cref="GetRouteValues" />
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
            var value = lambda.Compile().DynamicInvoke();
            return new Dictionary<string, object>
            {
                { parameterInfo.Name, value }
            };
        }
    }
}
