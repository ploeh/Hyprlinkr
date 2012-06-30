using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using Ploeh.AutoFixture.Xunit;
using Ploeh.Hyprlinkr;
using Xunit;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class RoupleTests
    {
        [Theory, AutoHypData]
        public void RouteNameIsCorrect(
            [Frozen]IDictionary<string, object> dummyFrozenBeforeStringIsFrozen,
            [Frozen]string expected,
            Rouple sut)
        {
            Assert.Equal<string>(expected, sut.RouteName);
        }

        [Theory, AutoHypData]
        public void RouteValuesIsCorrect(
            [Frozen]IDictionary<string, object> expected,
            Rouple sut)
        {
            Assert.Equal<IDictionary<string, object>>(expected, sut.RouteValues);
        }
    }
}
