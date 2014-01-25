using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using Xunit;
using Ploeh.Hyprlinkr;
using System.Linq.Expressions;
using Ploeh.Hyprlinkr.UnitTest.Controllers;
using Ploeh.AutoFixture.Xunit;
using System.Net.Http;
using System.Web.Http;
using Moq;
using System.Reflection;
using Ploeh.AutoFixture.Idioms;
using Moq.Protected;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class RouteLinkerTests
    {
        [Theory, AutoHypData]
        public void ConstructorHasAppropriateGuards(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(RouteLinker).GetConstructors());
        }

        [Theory, AutoHypData]
        public void SutIsRouteLinker(RouteLinker sut)
        {
            Assert.IsAssignableFrom<IResourceLinker>(sut);
        }

        [Theory, AutoHypData]
        public void GetNullThrows(RouteLinker sut)
        {
            Assert.Throws<ArgumentNullException>(() =>
                sut.GetUri<Version>(null));
        }

        [Theory, AutoHypData]
        public void GetUriFromInvalidExpressionThrows(RouteLinker sut)
        {
            Expression<Action<object>> expressionWhichIsNotAMethodCall =
                _ => new object();
            Assert.Throws<ArgumentException>(() =>
                sut.GetUri<object>(expressionWhichIsNotAMethodCall));
        }

        [Theory, AutoHypData]
        public void GetFooRouteForDefaultGetMethodReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut)
        {
            request.AddDefaultRoute();

            Uri actual = sut.GetUri<FooController>(r => r.GetDefault());

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/foo");
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetBarRouteForDefaultGetMethodReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut)
        {
            request.AddDefaultRoute();

            Uri actual = sut.GetUri<BarController>(r => r.GetDefault());

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/bar");
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriForGetMethodWithParameters(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            int id)
        {
            request.AddDefaultRoute();

            var actual = sut.GetUri<FooController>(r => r.GetById(id));

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/foo/" + id);
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriForGetMethodWithNamedParametersReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            int ploeh,
            string fnaah)
        {
            request.AddDefaultRoute();

            var actual = sut.GetUri<FooController>(r =>
                r.GetWithPloehAndFnaah(ploeh, fnaah));

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected =
                new Uri(
                    new Uri(baseUri),
                    "api/foo?ploeh=" + ploeh + "&fnaah=" + fnaah);
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriWithCustomRouteAndDispatcherReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            [Frozen(As = typeof(IRouteValuesQuery))]ScalarRouteValuesQuery dummyQuery,
            [Frozen]Mock<IRouteDispatcher> dispatcherStub,
            string routeName,
            [Greedy]RouteLinker sut,
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

            // Act
            var actual = sut.GetUri<FooController>(r =>
                r.GetWithPloehAndFnaah(ploeh, fnaah));

            // Assert
            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected =
                new Uri(
                    new Uri(baseUri),
                    "foo/" + ploeh + "/" + fnaah);
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GivenModestConstructorRequestIsCorrect(
            [Frozen]HttpRequestMessage expected,
            [Modest]RouteLinker sut)
        {
            Assert.Equal<HttpRequestMessage>(expected, sut.Request);
        }

        [Theory, AutoHypData]
        public void GivenModestConstructorQueryIsCorrect(
            [Modest]RouteLinker sut)
        {
            Assert.IsAssignableFrom<ScalarRouteValuesQuery>(
                sut.RouteValuesQuery);
        }

        [Theory, AutoHypData]
        public void GivenGreedyConstructorRequestIsCorrect(
            [Frozen]HttpRequestMessage expected,
            [Greedy]RouteLinker sut)
        {
            Assert.Equal<HttpRequestMessage>(expected, sut.Request);
        }

        [Theory, AutoHypData]
        public void GivenGreedyConstructorDispatcherIsCorrect(
            [Frozen]IRouteDispatcher expected,
            [Greedy]RouteLinker sut)
        {
            Assert.Equal<IRouteDispatcher>(expected, sut.RouteDispatcher);
        }

        [Theory, AutoHypData]
        public void GivenGreedyConstructorQueryIsCorrect(
            [Frozen]IRouteValuesQuery expected,
            [Greedy]RouteLinker sut)
        {
            Assert.Equal(expected, sut.RouteValuesQuery);
        }

        [Theory, AutoHypData]
        public void GetFooRouteForDefaultGetMethodFromIndexedUriReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            string currentId)
        {
            request.RequestUri = new Uri(request.RequestUri, "api/foo/" + currentId);
            request.AddRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    controller = "Home",
                    id = RouteParameter.Optional
                });

            Uri actual = sut.GetUri<FooController>(r => r.GetDefault());

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/foo");
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriWhenRequestHasNoRouteDataThrowsArgumentException([Frozen]HttpRequestMessage request,
            RouteLinker sut)
        {
            request.RequestUri = new Uri(request.RequestUri, "api/foo/");

            Assert.Throws<InvalidOperationException>(() => sut.GetUri<FooController>(r => r.GetDefault()));
        }

        [Theory, AutoHypData]
        public void GetUriDoesNotMutateExistingRouteData(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut)
        {
            request.AddDefaultRoute();
            var expected = new HashSet<KeyValuePair<string, object>>(
                request.GetRouteData().Values);

            sut.GetUri<FooController>(r => r.GetDefault());

            var actual = request.GetRouteData().Values.ToList();
            Assert.True(expected.SetEquals(actual));
        }

        [Theory, AutoHypData]
        public void GetUriFromBaseActionMethodReturnsCorrectResponse(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut)
        {
            request.AddDefaultRoute();

            var actual = sut.GetUri<DerivedController>(c => c.BaseMethod());

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/derived");
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriFromModelControllerReturnsCorrectResponse(
            [Frozen]HttpRequestMessage request,
            [Frozen]Mock<IRouteValuesQuery> queryStub,
            [Frozen(As = typeof(IRouteDispatcher))]DefaultRouteDispatcher dummy,
            [Greedy]RouteLinker sut,
            SomeModel model)
        {
            request.AddDefaultRoute();
            queryStub.SetReturnsDefault<IDictionary<string, object>>(
                new Dictionary<string, object>
                {
                    { "number", model.Number.ToString() },
                    { "text", model.Text }
                });

            var actual = sut.GetUri<ModelController>(c => c.Get(model));

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(
                new Uri(baseUri),
                "api/model?number=" + model.Number + "&text=" + model.Text);
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriForNonMatchingRouteThrows(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            int id)
        {
            request.RequestUri = new Uri(request.RequestUri, "api/foo/" + id);
            request.AddRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{bar}",
                defaults: new
                {
                    controller = "Home",
                    id = RouteParameter.Optional
                });

            Assert.Throws<InvalidOperationException>(() =>
                sut.GetUri<FooController>(c => c.GetById(id)));
        }

        [Theory, AutoHypData]
        public void GetUriForNullQueryParameterReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            int id)
        {
            request.AddDefaultRoute();

            var actual = sut.GetUri<BarController>(
                c => c.GetWithIdAndQueryParameter(id, null));

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/bar/" + id);
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriForNullFnaahReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            int ploeh)
        {
            request.AddDefaultRoute();

            var actual = sut.GetUri<FooController>(
                c => c.GetWithPloehAndFnaah(ploeh, null));

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/foo?ploeh=" + ploeh);
            Assert.Equal(expected, actual);
        }

        [Theory, AutoHypData]
        public void GetUriWithNullOptionalParameterReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            int id,
            string bar)
        {
            request.AddDefaultRoute();

            var actual = sut.GetUri<FooController>(
                c => c.GetWithIdAndOptionalParameter(id, bar, null));

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = 
                new Uri(new Uri(baseUri), "api/foo/" + id + "?bar=" + bar);
            Assert.Equal(expected, actual);
        }
    }
}
