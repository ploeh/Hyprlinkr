using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using Ploeh.Hyprlinkr;
using Xunit;
using System.Reflection;
using Ploeh.Hyprlinkr.UnitTest.Controllers;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class DefaultRouteDispatcherTests
    {
        [Theory, AutoHypData]
        public void SutIsRouteLinker(DefaultRouteDispatcher sut)
        {
            Assert.IsAssignableFrom<IRouteDispatcher>(sut);
        }

        [Theory, AutoHypData]
        public void DispatchReturnsResultWithCorrectRouteName(
            DefaultRouteDispatcher sut,
            MethodInfo method,
            IDictionary<string, object> routeValues)
        {
            var actual = sut.Dispatch(method, routeValues);
            Assert.Equal("API Default", actual.RouteName);
        }

        [Theory, AutoHypData]
        public void DispatchPreservesAllRouteValues(
            DefaultRouteDispatcher sut,
            MethodInfo method,
            IDictionary<string, object> routeValues)
        {
            var actual = sut.Dispatch(method, routeValues);

            var expected = new HashSet<KeyValuePair<string, object>>(routeValues);
            Assert.True(expected.IsSubsetOf(actual.RouteValues));
        }

        [Theory, AutoHypData]
        public void DispatchAddsFooControllerNameToRouteValues(
            DefaultRouteDispatcher sut,
            IDictionary<string, object> routeValues)
        {
            var method = Reflect<FooController>.GetMethod(c => c.GetDefault());
            var actual = sut.Dispatch(method, routeValues);
            Assert.Equal("foo", actual.RouteValues["controller"]);
        }

        [Theory, AutoHypData]
        public void DispatchAddsBarControllerNameToRouteValues(
            DefaultRouteDispatcher sut,
            IDictionary<string, object> routeValues)
        {
            var method = Reflect<BarController>.GetMethod(c => c.GetDefault());
            var actual = sut.Dispatch(method, routeValues);
            Assert.Equal("bar", actual.RouteValues["controller"]);
        }

        [Theory, AutoHypData]
        public void DispatchDoesNotMutateInputRouteValues(
            DefaultRouteDispatcher sut,
            MethodInfo method,
            IDictionary<string, object> routeValues)
        {
            var expected = routeValues.ToList();
            sut.Dispatch(method, routeValues);
            Assert.True(expected.SequenceEqual(routeValues));
        }
    }
}
