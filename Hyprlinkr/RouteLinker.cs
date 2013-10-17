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
using System.Globalization;
using System.Threading.Tasks;

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
    public class RouteLinker : IResourceLinker
    {
        private readonly HttpRequestMessage request;
        private readonly IRouteValuesQuery valuesQuery;
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
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteValuesQuery)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteValuesQuery, IRouteDispatcher)" />
        public RouteLinker(HttpRequestMessage request)
            : this(request, new DefaultRouteDispatcher())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteLinker" /> class.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="routeValuesQuery">
        /// A Strategy for extracting route values.
        /// </param>
        /// <remarks>
        /// <para>
        /// This constructor overload requires a custom
        /// <see cref="IRouteValuesQuery" />. If you don't want to use a custom
        /// query, you can use the simpler constructor overload.
        /// </para>
        /// <para>
        /// After initialization, the <paramref name="request" /> value is
        /// available through the <see cref="Request" /> property, and the
        /// <paramref name="routeValuesQuery" /> value is available via the
        /// <see cref="RouteValuesQuery" /> property.
        /// </para>
        /// </remarks>
        /// <seealso cref="RouteLinker(HttpRequestMessage)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteValuesQuery, IRouteDispatcher)" />
        public RouteLinker(HttpRequestMessage request, IRouteValuesQuery routeValuesQuery)
            : this(request, routeValuesQuery, new DefaultRouteDispatcher())
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
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteValuesQuery)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteValuesQuery, IRouteDispatcher)" />
        public RouteLinker(HttpRequestMessage request, IRouteDispatcher dispatcher)
            : this(request, new ScalarRouteValuesQuery(), dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteLinker" /> class.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="routeValuesQuery">
        /// A Strategy for extracting route values.
        /// </param>
        /// <param name="dispatcher">A custom dispatcher.</param>
        /// <remarks>
        /// <para>
        /// This constructor overload requires custom Strategies to be
        /// injected. If you don't want to supply one or both custom
        /// Strategies, you can use a simpler constructor overload.
        /// </para>
        /// <para>
        /// After initialization, the parameter values are available as
        /// read-only properties.
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// request is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// routeValuesQuery is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// dispatcher is null
        /// </exception>
        /// <seealso cref="RouteLinker(HttpRequestMessage)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteValuesQuery)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        /// <seealso cref="Request" />
        /// <see cref="RouteValuesQuery" />
        /// <see cref="RouteDispatcher" />
        public RouteLinker(
            HttpRequestMessage request,
            IRouteValuesQuery routeValuesQuery,
            IRouteDispatcher dispatcher)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (routeValuesQuery == null)
                throw new ArgumentNullException("routeValuesQuery");
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            this.request = request;
            this.valuesQuery = routeValuesQuery;
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Creates an URI based on a type-safe expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type of resource to link to. This will typically be the type of an
        /// <see cref="System.Web.Http.ApiController" />, but doesn't have to be.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The return type of the Action Method of the resource.
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
        /// routing configuration is consulted to build an appropriate URI which matches the Action
        /// Method. The routing configuration is pulled from the <see cref="HttpRequestMessage" />
        /// instance supplied to the constructor of the <see cref="RouteLinker" /> class.
        /// </para>
        /// <para>
        /// This overload mostly exists to support F# clients.
        /// </para>
        /// </remarks>
        /// <seealso cref="RouteLinker(HttpRequestMessage)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        /// <example>
        /// This example demonstrates how an F# client can create an <see cref="Uri" /> instance for a GetById
        /// method defined on a FooController class.
        /// <code>
        /// let uri = linker.GetUri(fun (c : FooController) -> c.GetById(1337))
        /// </code>
        /// Given the default API route configuration, the resulting URI will be something like
        /// this (assuming that the base URI is http://localhost): http://localhost/api/foo/1337
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The expression is strongly typed in order to prevent the caller from passing any sort of expression. It doesn't fully capture everything the caller might throw at it, but it does constrain the caller as well as possible. This enables the developer to get a compile-time exception instead of a run-time exception in most cases where an invalid expression is being supplied.")]
        public Uri GetUri<T, TResult>(Expression<Func<T, TResult>> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var methodCallExp = method.Body as MethodCallExpression;
            if (methodCallExp == null)
                throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "method");

            return this.GetUri(methodCallExp);
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
        /// routing configuration is consulted to build an appropriate URI which matches the Action
        /// Method. The routing configuration is pulled from the <see cref="HttpRequestMessage" />
        /// instance supplied to the constructor of the <see cref="RouteLinker" /> class.
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The expression is strongly typed in order to prevent the caller from passing any sort of expression. It doesn't fully capture everything the caller might throw at it, but it does constrain the caller as well as possible. This enables the developer to get a compile-time exception instead of a run-time exception in most cases where an invalid expression is being supplied.")]
        public Uri GetUri<T>(Expression<Action<T>> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var methodCallExp = method.Body as MethodCallExpression;
            if (methodCallExp == null)
                throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "method");

            return this.GetUri(methodCallExp);
        }

        /// <summary>
        /// Creates an URI based on a type-safe expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type of resource to link to. This will typically be the type of
        /// an <see cref="System.Web.Http.ApiController" />, but doesn't have
        /// to be.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The return type of the Action Method of the resource.
        /// </typeparam>
        /// <param name="method">
        /// An expression wich identifies the action method that serves the
        /// desired resource.
        /// </param>
        /// <returns>
        /// A <see cref="Task{Uri}" /> instance which represents the resource
        /// identifed by <paramref name="method" />.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is used to build valid URIs for resources represented
        /// by code. In the ASP.NET Web API, resources are served by Action
        /// Methods on Controllers. If building a REST service with hypermedia
        /// controls, you will want to create links to various other resources
        /// in your service. Viewed from code, these resources are encapsulated
        /// by Action Methods, but you need to build valid URIs that, when
        /// requested via HTTP, invokes the desired Action Method.
        /// </para>
        /// <para>
        /// The target Action Method can be type-safely identified by the
        /// <paramref name="method" /> expression.
        /// The <typeparamref name="T" /> type argument will typically indicate
        /// a particular class which derives from
        /// <see cref="System.Web.Http.ApiController" />, but there's no
        /// generic constraint on the type argument, so this is not required.
        /// </para>
        /// <para>
        /// Based on the Action Method identified by the supplied expression,
        /// the ASP.NET Web API routing configuration is consulted to build an
        /// appropriate URI which matches the Action Method. The routing
        /// configuration is pulled from the <see cref="HttpRequestMessage" />
        /// instance supplied to the constructor of the
        /// <see cref="RouteLinker" /> class.
        /// </para>
        /// <para>
        /// This overload supports extracting valid URI instances from async
        /// Controllers.
        /// </para>
        /// </remarks>
        /// <seealso cref="RouteLinker(HttpRequestMessage)" />
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        /// <seealso cref="GetUri{T}(Expression{Action{T}})" />
        /// <seealso cref="GetUri{T, TResult}(Expression{Func{T, TResult}})" />
        /// <exception cref="System.ArgumentNullException">method is null</exception>
        /// <exception cref="System.ArgumentException">The expression's body isn't a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().</exception>
        /// <example>
        /// This example demonstrates how to create an <see cref="Uri" />
        /// instance for a Get method defined on an AsyncController class.
        /// <code>
        /// Uri actual = linker.GetUriAsync((AsyncController c) => c.Get(id)).Result;
        /// </code>
        /// Given the default API route configuration, the resulting URI will
        /// be something like this (assuming that the base URI is
        /// http://localhost): http://localhost/api/async/1337
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The expression is strongly typed in order to prevent the caller from passing any sort of expression. It doesn't fully capture everything the caller might throw at it, but it does constrain the caller as well as possible. This enables the developer to get a compile-time exception instead of a run-time exception in most cases where an invalid expression is being supplied.")]
        public Task<Uri> GetUriAsync<T, TResult>(Expression<Func<T, Task<TResult>>> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var methodCallExp = method.Body as MethodCallExpression;
            if(methodCallExp == null)
                throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "method");

            return Task.Factory.StartNew(() => this.GetUri(methodCallExp));
        }

        private Uri GetUri(MethodCallExpression methodCallExp)
        {
            var r = this.Dispatch(methodCallExp);

            var relativeUri = this.GetRelativeUri(r);
            var baseUri = this.GetBaseUri();
            return new Uri(baseUri, relativeUri);
        }

        private Rouple Dispatch(MethodCallExpression methodCallExp)
        {
            var routeValues = this.valuesQuery.GetRouteValues(methodCallExp);
            return this.dispatcher.Dispatch(methodCallExp, routeValues);
        }

        #region CA suppressions
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetUri", Justification = "Workaround for a bug in CA: https://connect.microsoft.com/VisualStudio/feedback/details/521030/")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IDictionary", Justification = "Workaround for a bug in CA: https://connect.microsoft.com/VisualStudio/feedback/details/521030/")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UrlHelper", Justification = "Workaround for a bug in CA: https://connect.microsoft.com/VisualStudio/feedback/details/521030/")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RouteLinker", Justification = "Workaround for a bug in CA: https://connect.microsoft.com/VisualStudio/feedback/details/521030/")]
        #endregion
        private Uri GetRelativeUri(Rouple r)
        {
            var urlHelper = this.CreateUrlHelper();
            var relativeUri = urlHelper.Route(r.RouteName, r.RouteValues);
            if (relativeUri == null)
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The route string returned by System.Web.Http.Routing.UrlHelper.Route(string, IDictionary<string, object>) is null, which indicates an error. This can happen if the Action Method identified by the RouteLinker.GetUri method doesn't have a matching route with the name \"{0}\", or if the route parameter names don't match the method arguments.",
                        r.RouteName));

            return new Uri(relativeUri, UriKind.Relative);
        }

        private Uri GetBaseUri()
        {
            var authority =
                this.request.RequestUri.GetLeftPart(UriPartial.Authority);
            return new Uri(authority);
        }

        private UrlHelper CreateUrlHelper()
        {
            return this.CopyRequestWithoutRouteValues().GetUrlHelper();
        }

        private HttpRequestMessage CopyRequestWithoutRouteValues()
        {
            var requestCopy = new HttpRequestMessage(
                this.request.Method,
                this.request.RequestUri);
            try
            {
                CopyPropertiesOfCurrentRequestTo(requestCopy);
            }
            catch
            {
                requestCopy.Dispose();
                throw;
            }
            return requestCopy;
        }

        private void CopyPropertiesOfCurrentRequestTo(HttpRequestMessage destination)
        {
            CopyNonRouteValuePropertiesFromRequestToCopy(destination);
            CopyRouteDataToRequest(destination);
        }

        private void CopyNonRouteValuePropertiesFromRequestToCopy(HttpRequestMessage destination)
        {
            foreach (var kvp in this.request.Properties)
                if (kvp.Key != HttpPropertyKeys.HttpRouteDataKey)
                    destination.Properties.Add(kvp.Key, kvp.Value);
        }

        private void CopyRouteDataToRequest(HttpRequestMessage requestCopy)
        {
            var routeData = GetRouteDataOrThrowException();
            requestCopy.Properties.Add(
                HttpPropertyKeys.HttpRouteDataKey,
                new HttpRouteData(routeData.Route));
        }


        private IHttpRouteData GetRouteDataOrThrowException()
        {
            var routeData = this.request.GetRouteData();
            if (routeData == null)
                throw new InvalidOperationException("Current request has no route data.");
            return routeData;
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

        /// <summary>Gets the route values query.</summary>
        public IRouteValuesQuery RouteValuesQuery
        {
            get { return this.valuesQuery; }
        }

        /// <summary>
        /// Gets the route dispatcher.
        /// </summary>
        /// <seealso cref="RouteLinker(HttpRequestMessage, IRouteDispatcher)" />
        public IRouteDispatcher RouteDispatcher
        {
            get { return this.dispatcher; }
        }
    }
}
