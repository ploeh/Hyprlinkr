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
        public void LinkDoesNotMutateExistingRouteData(
            [Frozen]HttpRequestMessage request,
            UrlHelper sut)
        {
            request.AddDefaultRoute();
            var expected = new HashSet<KeyValuePair<string, object>>(
                request.GetRouteData().Values);

            sut.Link<FooController>(r => r.GetDefault());

            var actual = request.GetRouteData().Values.ToList();
            Assert.True(expected.SetEquals(actual));
        }

        [Theory, AutoHypData]
        public void LinkThrowsExceptionForNullExpression(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut)
        {
            request.AddDefaultRoute();
            Assert.Throws(typeof(ArgumentNullException), () => sut.Link<FooController>(null));
        }


        [Theory, AutoHypData]
        public void LinkForNoRouteDataThrows([Frozen]HttpRequestMessage request,
            UrlHelper sut)
        {
            request.RequestUri = new Uri(request.RequestUri, "api/foo/");
            Assert.Throws<InvalidOperationException>(() => sut.Link<FooController>(r => r.GetDefault()));
        }

        [Theory, AutoHypData]
        public void LinkFromBaseActionMethodReturnsCorrectResponse(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<DerivedController>(c => c.BaseMethod());
            var actual = sut.Link<DerivedController>(c => c.BaseMethod());

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void LinkReturnsCorrectUriForExpressionOfFuncWithSingleParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooController>(a => a.GetById(id));
            var actual = sut.Link<FooController>(a => a.GetById(id));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void LinkReturnsCorrectUriForExpressionOfActionWithSingleParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooController, object>(a => a.GetById(id));
            var actual = sut.Link<FooController, object>(a => a.GetById(id));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void LinkReturnsCorrectUriForExpressionOfFuncWithMultipleParameters(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id, int bar)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooBarController, object>(a => a.GetBar(id, bar));
            var actual = sut.Link<FooBarController, object>(a => a.GetBar(id, bar));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void LinkReturnsCorrectUriForExpressionOfActionWithMultipleParameters(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, int id, int bar)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<FooBarController>(a => a.GetBar(id, bar));
            var actual = sut.Link<FooBarController>(a => a.GetBar(id, bar));

            Assert.Equal(expected, actual);
        }


        [Theory, AutoHypData]
        public void LinkReturnsCorrectUriForExpressionOfActionWithComplexParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut,
            SomeModel someModel)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<ModelController>(a => a.Get(someModel));
            var actual = sut.Link<ModelController>(a => a.Get(someModel));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void LinkReturnsCorrectUriForExpressionOfFuncWithComplexParameter(
            [Frozen]HttpRequestMessage request,
            RouteLinker forComparison,
            UrlHelper sut, SomeModel someModel)
        {
            request.AddDefaultRoute();

            var expected = forComparison.GetUri<ModelController, object>(a => a.Get(someModel));
            var actual = sut.Link<ModelController, object>(a => a.Get(someModel));

            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void LinkWithCustomRouteAndDispatcherReturnsCorrectResult(
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
            var actual = sut.Link<FooController>(r =>
                r.GetWithPloehAndFnaah(ploeh, fnaah), dispatcherStub.Object);

            //Assert           
            Assert.Equal(expected, actual);
        }

    }
}
