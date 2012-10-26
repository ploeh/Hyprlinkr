using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Ploeh.Hyprlinkr
{
    /// <summary>Contains extension methods for the <see cref="ResourceLinkVerifier"/>.</summary>
    public static class ResourceLinkVerifierExtensions
    {
        /// <summary>
        ///     Parses the passed <see cref="Uri"/>, verifies that it matches the specified controller action and returns the controller action parameters in a dynamic
        ///     object.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="resourceLinkVerifier">The resource link verifier to use to parse and verify the URI.</param>
        /// <param name="uri">The URI to parse and verify.</param>
        /// <param name="expectedAction">The expected controller action.</param>
        /// <returns>A dynamic object with a property for each controller action parameter.</returns>
        /// <exception cref="System.Web.Http.HttpResponseException">
        ///     Thrown with status code <see cref="HttpStatusCode.BadRequest"/> if the <paramref name="uri"/> couldn't be parsed or if it didn't match the expected
        ///     controller action.
        /// </exception>
        public static dynamic ParseAndVerify<TController>(
            this ResourceLinkVerifier resourceLinkVerifier, Uri uri, Expression<Action<TController>> expectedAction)
        {
            if (resourceLinkVerifier == null)
                throw new ArgumentNullException("resourceLinkVerifier");

            HttpActionContext context;
            if (!resourceLinkVerifier.TryParse(uri, out context) || !resourceLinkVerifier.Verify(context, expectedAction))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            IDictionary<string, object> result = new ExpandoObject();
            foreach (var actionArgument in context.ActionArguments)
                result.Add(actionArgument);
            return result;
        }
    }
}
