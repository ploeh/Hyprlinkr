using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ploeh.Hyprlinkr
{
    internal static class ExpressionExtensions
    {
        internal static MethodCallExpression GetMethodCallExpression(
            this LambdaExpression expression)
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
    }
}
