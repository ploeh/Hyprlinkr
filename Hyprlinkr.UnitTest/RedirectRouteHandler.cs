using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace Ploeh.Hyprlinkr.UnitTest
{
    /// <summary>
    /// A <see cref="HttpMessageHandler"/> that redirects the caller to the new location of the resource.
    /// </summary>
    public class RedirectRouteHandler : HttpMessageHandler
    {
        private readonly bool permanently;
        private readonly IHttpRoute target;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectRouteHandler"/> class.
        /// </summary>
        /// <param name="target">
        /// The target route.
        /// </param>
        /// <param name="permanently">
        /// if set to <c>true</c>, the route has permanently been redirected.
        /// </param>
        public RedirectRouteHandler(IHttpRoute target, bool permanently)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
            this.permanently = permanently;
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">
        /// The HTTP request message to send.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to cancel operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var vpd = this.target.GetVirtualPath(request, request.GetRouteData().Values);
                    var response = new HttpResponseMessage(this.permanently ? HttpStatusCode.MovedPermanently : HttpStatusCode.Moved);
                    response.Headers.Location = new Uri(vpd.VirtualPath, UriKind.Relative);
                    return response;
                });
        }
    }
}
