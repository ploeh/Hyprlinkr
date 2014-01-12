using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    ///     <para> This class parses URIs into a structured representation. The <see cref="HttpActionContext"/> class is used as said representation. </para>
    ///     <para> Additionally, this class is capable of verifying that a <see cref="HttpActionContext"/> matches a specific controller action. </para>
    /// </summary>
    /// <remarks>
    /// Example: <code>
    /// <![CDATA[
    /// HttpContextAction contextAction;
    /// if(linkParser.TryParseUri(uri, out contextAction) && linkParser.Verify<SomeController>(x => x.SomeAction(Arg<int>.Any)))
    /// {
    ///     var id = (int)contextAction.ActionArguments["id"];
    /// }
    /// ]]>
    /// </code>
    /// </remarks>
    public class ResourceLinkParser : IResourceLinkParser, IActionVerifier
    {
        private readonly IHttpActionSelector actionSelector;
        private readonly HttpConfiguration configuration;
        private readonly IHttpControllerSelector controllerSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLinkParser"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration to use to parse the URIs.
        /// </param>
        public ResourceLinkParser(HttpConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.configuration = configuration;
            this.actionSelector = this.configuration.Services.GetActionSelector();
            this.controllerSelector = this.configuration.Services.GetHttpControllerSelector();
        }

        /// <summary>
        /// Gets the configuration used to parse the URIs.
        /// </summary>
        public HttpConfiguration Configuration
        {
            get { return this.configuration; }
        }

        /// <summary>
        /// Parses the specified URI.
        /// </summary>
        /// <param name="uri">
        /// The URI to parse.
        /// </param>
        /// <returns>
        /// The <see cref="HttpActionContext"/> with bound parameter values.
        /// </returns>
        /// <exception cref="System.ArgumentException">The URI is invalid or no action matches the specified URI.</exception>
        public HttpActionContext Parse(Uri uri)
        {
            HttpActionContext result;
            if (TryParse(uri, out result))
                return result;

            throw new ArgumentException("The URI is invalid or no action matches the specified URI");
        }

        /// <summary>
        /// Tries to parse the specified URI.
        /// </summary>
        /// <param name="uri">
        /// The URI to parse.
        /// </param>
        /// <param name="actionContext">
        /// The <see cref="HttpActionContext"/> with bound parameter values.
        /// </param>
        /// <returns>
        ///     <c>true</c> in case the URI could be parsed successfully and matched to an action; otherwise, <c>false</c> .
        /// </returns>
        public bool TryParse(Uri uri, out HttpActionContext actionContext)
        {
            actionContext = null;

            var controllerContext = GetControllerContext(uri);
            if (controllerContext == null)
                return false;

            var actionDescriptor = GetActionDescriptor(controllerContext);
            if (actionDescriptor == null)
                return false;

            actionContext = new HttpActionContext(controllerContext, actionDescriptor);
            actionDescriptor.ActionBinding.ExecuteBindingAsync(actionContext, CancellationToken.None).Wait();

            return true;
        }

        /// <summary>
        /// Verifies that the specified action context refers to the same controller action as the action specified by the expression.
        /// </summary>
        /// <typeparam name="TController">
        /// The type of the controller.
        /// </typeparam>
        /// <param name="actionContext">
        /// The action context to verify.
        /// </param>
        /// <param name="expectedAction">
        /// The expression defining the expected action.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified action context refers to the same controller action as the supplied expression; otherwise, <c>false</c> .
        /// </returns>
        public bool Verify<TController>(HttpActionContext actionContext, Expression<Action<TController>> expectedAction)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");
            if (expectedAction == null)
                throw new ArgumentNullException("expectedAction");

            if (typeof(TController) != actionContext.ControllerContext.ControllerDescriptor.ControllerType)
                return false;

            var expectedActionMethod = expectedAction.GetMethodCallExpression().Method;

            MethodInfo actualActionMethod;
            var actionDescriptor = actionContext.ActionDescriptor as ReflectedHttpActionDescriptor;
            if (actionDescriptor == null)
                actualActionMethod = GetActionMethod(actionContext.ActionDescriptor);
            else
                actualActionMethod = actionDescriptor.MethodInfo;

            return actualActionMethod.MethodHandle.Equals(expectedActionMethod.MethodHandle);
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> object the supplied <see cref="HttpActionDescriptor"/> describes.
        /// </summary>
        /// <param name="actionDescriptor">
        /// The action descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="MethodInfo"/> object the supplied <see cref="HttpActionDescriptor"/> describes or <c>null</c> if no matching method is found.
        /// </returns>
        private static MethodInfo GetActionMethod(HttpActionDescriptor actionDescriptor)
        {
            var actionParameters =
                actionDescriptor.ActionBinding.ParameterBindings.Select(x => new { Name = x.Descriptor.ParameterName, x.Descriptor.ParameterType })
                                .OrderBy(x => x.Name);

            return
                actionDescriptor.ControllerDescriptor.ControllerType.GetMethods()
                                .Where(x => x.Name == actionDescriptor.ActionName)
                                .SingleOrDefault(
                                    x => x.GetParameters().Select(y => new { y.Name, y.ParameterType }).OrderBy(y => y.Name).SequenceEqual(actionParameters));
        }

        /// <summary>
        /// Gets the name of the controller for the supplied route data.
        /// </summary>
        /// <param name="routeData">
        /// The route data.
        /// </param>
        /// <returns>
        /// The name of the controller or <c>null</c> if the route data doesn't contain the name of the controller.
        /// </returns>
        private static string GetControllerName(IHttpRouteData routeData)
        {
            if (routeData == null)
                throw new ArgumentNullException("routeData");
            object tmp;
            if (routeData.Values.TryGetValue("controller", out tmp))
                return (string)tmp;

            return null;
        }

        /// <summary>
        /// Gets the action descriptor.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <returns>
        /// The action descriptor for the controller context.
        /// </returns>
        private HttpActionDescriptor GetActionDescriptor(HttpControllerContext controllerContext)
        {
            try
            {
                return this.actionSelector.SelectAction(controllerContext);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="HttpControllerContext"/> for the supplied <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">
        /// The URI.
        /// </param>
        /// <returns>
        /// The <see cref="HttpControllerContext"/> instance for the supplier URI.
        /// </returns>
        private HttpControllerContext GetControllerContext(Uri uri)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                var routeData = Configuration.Routes.GetRouteData(request);
                if (routeData == null)
                    return null;

                request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
                request.Properties[HttpPropertyKeys.HttpConfigurationKey] = Configuration;

                var controllerContext = new HttpControllerContext(Configuration, routeData, request);

                var controllerName = GetControllerName(routeData);

                if (!this.controllerSelector.GetControllerMapping().ContainsKey(controllerName))
                    return null;

                controllerContext.ControllerDescriptor = this.controllerSelector.SelectController(request);
                return controllerContext;
            }
        }
    }
}
