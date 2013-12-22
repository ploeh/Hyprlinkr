using System;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Ploeh.Hyprlinkr
{
    /// <summary>
    /// A class with extension methods for UrlHelper
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Returns URI matching helper's request and expression using RouteLinker's default route dispatcher
        /// </summary>
        /// <typeparam name="T">A class that derives from ApiController</typeparam>
        /// <typeparam name="TResult">Any result type</typeparam>
        /// <param name="helper">Provides the requested URI via helper.Request</param>
        /// <param name="expression">Method call expression of T</param>
        /// <returns>URI for the request and controller expression. </returns>
        public static Uri Link<T, TResult>(this UrlHelper helper, Expression<Func<T, TResult>> expression)
            where T : ApiController
        {
            var linker = new RouteLinker(helper.Request);

            return linker.GetUri(expression);
        }

        /// <summary>
        /// Returns URI matching helper's request and expression using RouteLinker's default route dispatcher.          
        /// </summary>
        /// <typeparam name="T">A class that derives from ApiController</typeparam>        
        /// <param name="helper">Provides the requested URI via helper.Request</param>
        /// <param name="expression">Expression of T</param>
        /// <returns>URI for the request and controller expression. </returns>
        public static Uri Link<T>(this UrlHelper helper, Expression<Action<T>> expression)
            where T : ApiController
        {
            var linker = new RouteLinker(helper.Request);

            return linker.GetUri(expression);
        }

        /// <summary>
        /// Returns URI matching helper's request and expression using provided route dispatcher         
        /// </summary>
        /// <typeparam name="T">A class that derives from ApiController</typeparam>        
        /// <param name="helper">Provides the requested URI via helper.Request</param>
        /// <param name="expression">Expression of T</param>
        /// <param name="dispatcher">Custom route dispatcher to use in place of default dispatcher</param>
        /// <returns>URI for the request and controller expression. </returns>
        public static Uri Link<T, TResult>(this UrlHelper helper, 
                                           Expression<Func<T, TResult>> expression, 
                                           IRouteDispatcher dispatcher)
            where T : ApiController
        {
            var linker = new RouteLinker(helper.Request, dispatcher);

            return linker.GetUri(expression);
        }

        /// <summary>
        /// Returns URI matching helper's request and expression using provided route dispatcher.          
        /// </summary>
        /// <typeparam name="T">A class that derives from ApiController</typeparam>        
        /// <param name="helper">Provides the requested URI via helper.Request</param>
        /// <param name="expression">Expression of T</param>
        /// <param name="dispatcher">Custom route dispatcher to use in place of default dispatcher</param>
        /// <returns>URI for the request and controller expression. </returns>
        public static Uri Link<T>(this UrlHelper helper, 
                                  Expression<Action<T>> expression, 
                                  IRouteDispatcher dispatcher)
           where T : ApiController
        {
            var linker = new RouteLinker(helper.Request, dispatcher);

            return linker.GetUri(expression);
        }
    }
}
