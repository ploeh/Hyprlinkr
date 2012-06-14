using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ploeh.Hyprlinkr
{
    public class RouteLinker
    {
        public void GetUri<T>(object method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "method");
        }
    }
}
