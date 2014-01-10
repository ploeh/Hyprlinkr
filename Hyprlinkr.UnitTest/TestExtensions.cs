using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http.Controllers;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public static class TestExtensions
    {
        public static HttpActionContext ToActionContext<TController>(this Expression<Action<TController>> expression)
        {
            var bodyCallExpression = ((MethodCallExpression)expression.Body);
            var methodInfo = bodyCallExpression.Method;
            var controllerDescriptor = new HttpControllerDescriptor { ControllerType = typeof(TController) };
            var actionContext =
                new HttpActionContext(
                    new HttpControllerContext { ControllerDescriptor = controllerDescriptor }, 
                    new ReflectedHttpActionDescriptor(controllerDescriptor, methodInfo));

            var parameters = bodyCallExpression.Method.GetParameters().ToDictionary(
                p => p.Name, bodyCallExpression.GetParameterValue);

            foreach (var kvp in parameters)
                actionContext.ActionArguments.Add(kvp.Key, kvp.Value);

            return actionContext;
        }

        private static object GetParameterValue(
            this MethodCallExpression methodCallExpression,
            ParameterInfo parameterInfo)
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
