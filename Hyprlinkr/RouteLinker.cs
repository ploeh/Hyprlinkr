using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.Http.Hosting;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    /// Creates URIs from type-safe expressions, based on routing configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The purpose of this class is to create correct URIs to other resources within an ASP.NET
    /// Web API solution. Instead of hard-coding URIs or building them from hard-coded URI
    /// templates which may go out of sync with the routes defined in an
    /// <see cref="System.Web.Http.HttpRouteCollection" />, the RouteLinker class provides a method
    /// where URIs can be built from the routes defined in the route collection.
    /// </para>
    /// </remarks>
    /// <seealso cref="GetUri{T}(Expression{Action{T}})" />
    public class RouteLinker : IResourceLinker, IDisposable
    {
        private readonly HttpRequestMessage request;
        private readonly IRouteDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteLinker"/> class.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <remarks>
        /// <para>
        /// After initialization, the <paramref name="request" /> value is available through the
        /// <see cref="Request" /> property.
        /// </para>
        /// </remarks>
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        public RouteLinker(HttpRequestMessage request)
            : this(request, new DefaultRouteDispatcher())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteLinker"/> class.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="dispatcher">A custom dispatcher.</param>
        /// <remarks>
        /// <para>
        /// This constructor overload requires a custom <see cref="IRouteDispatcher" />. If you
        /// don't want to use a custom dispatcher, you can use the simpler constructor overload.
        /// </para>
        /// <para>
        /// After initialization, the <paramref name="request" /> value is available through the
        /// <see cref="Request" /> property; and the <paramref name="dispatcher" /> is available
        /// through the <see cref="RouteDispatcher" /> property.
        /// </para>
        /// </remarks>
        /// <seealso cref="RouteLinker(HttpRequestMessage)" />
        public RouteLinker(HttpRequestMessage request, IRouteDispatcher dispatcher)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            this.request = request;
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Creates an URI based on a type-safe expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type of resource to link to. This will typically be the type of an
        /// <see cref="System.Web.Http.ApiController" />, but doesn't have to be.
        /// </typeparam>
        /// <param name="method">
        /// An expression wich identifies the action method that serves the desired resource.
        /// </param>
        /// <returns>
        /// An <see cref="Uri" /> instance which represents the resource identifed by
        /// <paramref name="method" />.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is used to build valid URIs for resources represented by code. In the
        /// ASP.NET Web API, resources are served by Action Methods on Controllers. If building a
        /// REST service with hypermedia controls, you will want to create links to various other
        /// resources in your service. Viewed from code, these resources are encapsulated by Action
        /// Methods, but you need to build valid URIs that, when requested via HTTP, invokes the
        /// desired Action Method.
        /// </para>
        /// <para>
        /// The target Action Method can be type-safely identified by the
        /// <paramref name="method" /> expression. The <typeparamref name="T" /> type argument will
        /// typically indicate a particular class which derives from
        /// <see cref="System.Web.Http.ApiController" />, but there's no generic constraint on the
        /// type argument, so this is not required.
        /// </para>
        /// <para>
        /// Based on the Action Method identified by the supplied expression, the ASP.NET Web API
        /// routing configuration is consulted to build an apprpriate URI which matches the Action
        /// Method. The routing configuration is pulled from the <see cref="HttpRequestMessage" />
        /// instance supplied to the constructor of the <see cref="RouteLinker" /> class. This is
        /// done by utilizing the
        /// <see cref="System.Net.Http.HttpRequestMessageExtensions.GetConfiguration(HttpRequestMessage)" />
        /// and
        /// <see cref="System.Net.Http.HttpRequestMessageExtensions.GetRouteData(HttpRequestMessage)" />
        /// extension methods.
        /// </para>
        /// </remarks>
        /// <seealso cref="RouteLinker(HttpRequestMessage)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        /// <example>
        /// This example demonstrates how to create an <see cref="Uri" /> instance for a GetById
        /// method defined on a FooController class.
        /// <code>
        /// var uri = linker.GetUri&lt;FooController&gt;(r => r.GetById(1337));
        /// </code>
        /// Given the default API route configuration, the resulting URI will be something like
        /// this (assuming that the base URI is http://localhost): http://localhost/api/foo/1337
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The expression is a strongly typed in order to prevent the caller from passing any sort of expression. It doesn't fully capture everything the caller might throw at it, but it does constrain the caller as well as possible. This enables the developer to get a compile-time exception instead of a run-time exception in most cases where an invalid expression is being supplied.")]
        public Uri GetUri<T>(Expression<Action<T>> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var methodCallExp = method.Body as MethodCallExpression;
            if (methodCallExp == null)
                throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "method");

            var routeValues = methodCallExp.Method.GetParameters()
                .ToDictionary(p => p.Name, p => GetValue(methodCallExp, p));
            var r = this.dispatcher.Dispatch(methodCallExp.Method, routeValues);

            var urlHelper = this.CreateUrlHelper();
            var relativeUri = new Uri(
                urlHelper.Route(r.RouteName, r.RouteValues),
                UriKind.Relative);

            var authority =
                this.request.RequestUri.GetLeftPart(UriPartial.Authority);
            var baseUri = new Uri(authority);
            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// Gets the request that this instance uses to create URIs.
        /// </summary>
        /// <seealso cref="RouteLinker(HttpRequestMessage)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        public HttpRequestMessage Request
        {
            get { return this.request; }
        }

        /// <summary>
        /// Gets the route dispatcher.
        /// </summary>
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        public IRouteDispatcher RouteDispatcher
        {
            get { return this.dispatcher; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This implementation follows the standard Dispose pattern by delegating to the virtual
        /// <see cref="Dispose(bool)" /> method.
        /// </para>
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        /// <remarks>
        /// <para>
        /// If <paramref name="disposing" /> is <see langword="true" /> this method disposes
        /// <see cref="Request" />.
        /// </para>
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.request.Dispose();
        }

        private static object GetValue(MethodCallExpression methodCallExp,
            ParameterInfo p)
        {
            var arg = methodCallExp.Arguments[p.Position];
            var lambda = Expression.Lambda(arg);
            return lambda.Compile().DynamicInvoke().ToString();
        }

        private UrlHelper CreateUrlHelper()
        {
            var routeData = this.request.GetRouteData();

            var req = new HttpRequestMessage(this.request.Method, this.request.RequestUri);
            foreach (var kvp in this.request.Properties)
            {
                if (kvp.Key != HttpPropertyKeys.HttpRouteDataKey)
                    req.Properties.Add(kvp.Key, kvp.Value);
            }
            req.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, new HttpRouteData(routeData.Route));

            var urlHelper = new UrlHelper(req);
            return urlHelper;
        }
    }
}
