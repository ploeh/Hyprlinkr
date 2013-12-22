using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Routing;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Ploeh.Hyprlinkr.UnitTest.Controllers;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class UrlHelperExtensionsTests
    {
        [Theory, AutoHypData]
        public void GetLinkDoesNotMutateExistingRouteData(
            [Frozen]HttpRequestMessage request,
            UrlHelper sut)
        {
            request.AddDefaultRoute();
            var expected = new HashSet<KeyValuePair<string, object>>(
                request.GetRouteData().Values);

            sut.GetLink<FooController>(r => r.GetDefault());

            var actual = request.GetRouteData().Values.ToList();
            Assert.True(expected.SetEquals(actual));
        }

        [Theory, AutoHypData]
        public void GetLinkThrowsExceptionForNullExpression(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut)
        {
            request.AddDefaultRoute();
            Assert.Throws(typeof(ArgumentNullException), () => sut.GetLink<FooController>(null));
        }


        [Theory, AutoHypData]
        public void GetLinkForNoRouteDataThrows([Frozen]HttpRequestMessage request,
            UrlHelper sut)
        {
            request.RequestUri = new Uri(request.RequestUri, "api/foo/");
            Assert.Throws<InvalidOperationException>(() => sut.GetLink<FooController>(r => r.GetDefault()));
        }

        [Theory, AutoHypData]
        public void GetLinkFromBaseActionMethodReturnsCorrectResponse(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<DerivedController>(c => c.BaseMethod());
            var actual = sut.GetLink<DerivedController>(c => c.BaseMethod());

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetLinkReturnsCorrectUriForExpressionOfFuncWithSingleParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooController>(a => a.GetById(id));
            var actual = sut.GetLink<FooController>(a => a.GetById(id));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetLinkReturnsCorrectUriForExpressionOfActionWithSingleParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooController, object>(a => a.GetById(id));
            var actual = sut.GetLink<FooController, object>(a => a.GetById(id));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetLinkReturnsCorrectUriForExpressionOfFuncWithMultipleParameters(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id, int bar)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooBarController, object>(a => a.GetBar(id, bar));
            var actual = sut.GetLink<FooBarController, object>(a => a.GetBar(id, bar));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetLinkReturnsCorrectUriForExpressionOfActionWithMultipleParameters(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id, int bar)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooBarController>(a => a.GetBar(id, bar));
            var actual = sut.GetLink<FooBarController>(a => a.GetBar(id, bar));

            Assert.Equal(expected, actual);
        }


        [Theory, AutoHypData]
        public void GetLinkReturnsCorrectUriForExpressionOfActionWithComplexParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut,
            SomeModel someModel)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<ModelController>(a => a.Get(someModel));
            var actual = sut.GetLink<ModelController>(a => a.Get(someModel));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetLinkReturnsCorrectUriForExpressionOfFuncWithComplexParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, SomeModel someModel)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<ModelController, object>(a => a.Get(someModel));
            var actual = sut.GetLink<ModelController, object>(a => a.Get(someModel));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetLinkWithCustomRouteAndDispatcherReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            [Frozen(As = typeof(IRouteValuesQuery))]ScalarRouteValuesQuery dummyQuery,
            [Frozen]Mock<IRouteDispatcher> dispatcherStub,
            string routeName,
            [Greedy]RouteLinker forComparison,
            UrlHelper sut,
            int ploeh,
            string fnaah)
        {
            // Arrange
            request.AddDefaultRoute();
            request.AddRoute(
                name: routeName,
                routeTemplate: "foo/{ploeh}/{fnaah}",
                defaults: new { });

            var method = Reflect<FooController>
                .GetMethod(c => c.GetWithPloehAndFnaah(ploeh, fnaah));
            dispatcherStub
                .Setup(d =>
                    d.Dispatch(
                        It.Is<MethodCallExpression>(exp => method.Equals(exp.Method)),
                        It.IsAny<IDictionary<string, object>>()))
                .Returns((MethodCallExpression _, IDictionary<string, object> routeValues) =>
                    new Rouple(routeName, routeValues));

            var expected = forComparison.GetUri<FooController>(r =>
              r.GetWithPloehAndFnaah(ploeh, fnaah));

            // Act
            var actual = sut.GetLink<FooController>(r =>
                r.GetWithPloehAndFnaah(ploeh, fnaah), dispatcherStub.Object);

            //Assert           
            Assert.Equal(expected, actual);
        }

    }
}
