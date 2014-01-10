using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    /// A class with some extension methods based around expressions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Gets the <see cref="MethodCallExpression"/> of the body of the supplied expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="MethodCallExpression"/> of the body of the supplied expression.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> doesn't represent an Action that invokes a method.</exception>
        internal static MethodCallExpression GetBodyMethodCallExpression(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException(
                    "The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().",
                    "expression");
            }

            return methodCallExpression;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> object of the body of the supplied expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="MethodInfo"/> object of the body of the supplied expression.
        /// </returns>
        public static MethodInfo GetMethodInfo(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return expression.GetBodyMethodCallExpression().Method;
        }

        /// <summary>
        /// Gets the parameter value of the specified parameter from the <see cref="MethodCallExpression"/> .
        /// </summary>
        /// <param name="methodCallExpression">
        /// The method call expression.
        /// </param>
        /// <param name="parameterInfo">
        /// The parameter to return the value for.
        /// </param>
        /// <returns>
        /// The value of the parameter.
        /// </returns>
        public static object GetParameterValue(this MethodCallExpression methodCallExpression, ParameterInfo parameterInfo)
        {
            if (methodCallExpression == null)
                throw new ArgumentNullException("methodCallExpression");
            if (parameterInfo == null)
                throw new ArgumentNullException("parameterInfo");

            var arg = methodCallExpression.Arguments[parameterInfo.Position];
            var lambda = Expression.Lambda(arg);
            return lambda.Compile().DynamicInvoke();
        }
    }
}
