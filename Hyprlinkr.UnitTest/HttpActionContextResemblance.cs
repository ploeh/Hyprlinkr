using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class HttpActionContextResemblance : HttpActionContext, IEqualityComparer<HttpActionContext>
    {
        private static readonly Lazy<IEqualityComparer<HttpActionContext>> _equalityComparer =
            new Lazy<IEqualityComparer<HttpActionContext>>(() => new HttpActionContextResemblance());

        public HttpActionContextResemblance(HttpActionContext actionContext)
            : base(actionContext.ControllerContext, actionContext.ActionDescriptor)
        {
            foreach (var kvp in actionContext.ActionArguments)
                ActionArguments.Add(kvp.Key, kvp.Value);

            foreach (var kvp in actionContext.ModelState)
                ModelState.Add(kvp);

            Response = actionContext.Response;
        }

        private HttpActionContextResemblance()
        {
        }

        public static IEqualityComparer<HttpActionContext> EqualityComparer
        {
            get { return _equalityComparer.Value; }
        }

        public bool Equals(HttpActionContext other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return ControllerContext.ControllerDescriptor.ControllerType
                   == other.ControllerContext.ControllerDescriptor.ControllerType
                   && ActionDescriptor.ActionName == other.ActionDescriptor.ActionName
                   && ActionArguments.OrderBy(x => x.Key).SequenceEqual(other.ActionArguments.OrderBy(x => x.Key));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HttpActionContext);
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns><see langword="true" /> if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(HttpActionContext x, HttpActionContext y)
        {
            var resemblance = x as HttpActionContextResemblance;
            if (resemblance != null)
                return resemblance.Equals(y);
            return y.Equals(x);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
        /// </exception>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(HttpActionContext obj)
        {
            return obj.GetHashCode();
        }
    }
}
