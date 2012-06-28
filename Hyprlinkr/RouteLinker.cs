using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;

namespace Ploeh.Hyprlinkr
{
    public class RouteLinker
    {
        private readonly HttpRequestMessage request;

        public RouteLinker(HttpRequestMessage request)
        {
            this.request = request;
        }

        public Uri GetUri<T>(Expression<Action<T>> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var methodCallExp = method.Body as MethodCallExpression;
            if (methodCallExp == null)
                throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "method");
            
            var routeValues = methodCallExp.Method.GetParameters()
                .ToDictionary(p => p.Name, p => GetValue(methodCallExp, p));

            var controllerName = methodCallExp
                .Method
                .ReflectedType
                .Name
                .ToLowerInvariant()
                .Replace("controller", "");
            routeValues.Add("controller", controllerName);

            var ctx = new HttpControllerContext(
                request.GetConfiguration(), request.GetRouteData(), request);
            var relativeUri = ctx.Url.Route("API Default", routeValues);

            var authority = 
                this.request.RequestUri.GetLeftPart(UriPartial.Authority);
            var baseUri = new Uri(authority);
            return new Uri(baseUri, relativeUri);
        }

        private static object GetValue(MethodCallExpression methodCallExp,
            ParameterInfo p)
        {
            var arg = methodCallExp.Arguments[p.Position];
            var lambda = Expression.Lambda(arg);
            return lambda.Compile().DynamicInvoke().ToString();
        }

    }
}
