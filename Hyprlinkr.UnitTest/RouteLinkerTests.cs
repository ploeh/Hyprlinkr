using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using Xunit;
using Ploeh.Hyprlinkr;
using System.Linq.Expressions;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class RouteLinkerTests
    {
        [Theory, AutoHypData]
        public void GetNullThrows(RouteLinker sut)
        {
            Assert.Throws<ArgumentNullException>(() =>
                sut.GetUri<Version>(null));
        }

        [Theory, AutoHypData]
        public void GetUriFromInvalidExpressionThrows(RouteLinker sut)
        {
            Expression<Action<object>> expressionWithIsNotAMethodCall = _ => new object();
            Assert.Throws<ArgumentException>(() =>
                sut.GetUri<object>(expressionWithIsNotAMethodCall));
        }
    }
}
