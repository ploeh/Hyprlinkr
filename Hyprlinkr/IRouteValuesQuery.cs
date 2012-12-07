using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    /// Represents a query which projects an expression's parameter values into
    /// a dictionary.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="MethodCallExpression" /> represents a call to a method. If
    /// the method takes arguments, the expression contains these arguments.
    /// The <see cref="GetRouteValues" /> method turns the argument values into
    /// strings and builds a dictionary of them.
    /// </para>
    /// </remarks>
    public interface IRouteValuesQuery
    {
        /// <summary>Gets the route values.</summary>
        /// <param name="methodCallExpression">A method call expression.</param>
        /// <returns>
        /// A dictionary of the method's arguments converted into strings.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Note to implementers:
        /// </para>
        /// <para>
        /// A <see cref="MethodCallExpression" /> represents a call to a
        /// method. If the method takes arguments, the expression contains
        /// these arguments. The GetRouteValues method turns the argument
        /// values into strings and builds a dictionary of them.
        /// </para>
        /// </remarks>
        IDictionary<string, object> GetRouteValues(
            MethodCallExpression methodCallExpression);
    }
}
