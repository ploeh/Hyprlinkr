using System;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Ploeh.AutoFixture.Idioms;
using Ploeh.Hyprlinkr.UnitTest.Controllers;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class ResourceLinkParserTests
    {
        [Theory]
        [AutoHypData]
        public void ConstructorHasAppropriateGuards(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ResourceLinkParser).GetConstructors());
        }

        [Theory]
        [AutoHypData]
        public void ParseUriFollowsRedirect(ResourceLinkParser sut, string host)
        {
            var defaultRoute = sut.Configuration.AddDefaultRoute();
            sut.Configuration.AddPermanentRedirectRoute("Redirect", new HttpRoute("api_old/{controller}/{id}", new HttpRouteValueDictionary(new { id = RouteParameter.Optional })), defaultRoute);
            var uri = new Uri(string.Format("http://{0}/api_old/bar", host));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<BarController>(x => x.GetDefault());

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForComplexRoute(ResourceLinkParser sut, string host, int id, int bar)
        {
            sut.Configuration.AddDefaultRoute();
            sut.Configuration.AddRoute("Complex Route", "api/{controller}/{id}/bars/{bar}", null);
            var uri = new Uri(string.Format("http://{0}/api/foobar/{1}/bars/{2}", host, id, bar));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<FooBarController>(x => x.GetBar(id, bar));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForUriWithOmittedOptionalParameters(
            ResourceLinkParser sut, string host, int id, string bar)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/foo/{1}?bar={2}", host, id, bar));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<FooController>(x => x.GetWithIdAndOptionalParameter(id, bar, "foo"));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForUriWithOverParametrization(
            ResourceLinkParser sut, string host, int id, string bar, string foo)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/bar/{1}?foo={2}&bar={3}", host, id, foo, bar));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<BarController>(x => x.GetWithIdAndQueryParameter(id, bar));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForUriWithQueryParameters(
            ResourceLinkParser sut, string host, int id, string bar)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/bar/{1}?bar={2}", host, id, bar));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<BarController>(x => x.GetWithIdAndQueryParameter(id, bar));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForUriWithQueryParametersInDifferentOrder(
            ResourceLinkParser sut, string host, int id, string bar, string foo)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/foo/{1}?foo={2}&bar={3}", host, id, foo, bar));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<FooController>(x => x.GetWithIdAndOptionalParameter(id, bar, foo));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForUriWithSuppliedOptionalParameters(
            ResourceLinkParser sut, string host, int id, string bar, string foo)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/foo/{1}?bar={2}&foo={3}", host, id, bar, foo));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<FooController>(x => x.GetWithIdAndOptionalParameter(id, bar, foo));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForUriWithoutQueryParameters(
            ResourceLinkParser sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/foo/{1}", host, id));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<FooController>(x => x.GetById(id));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithMultipleMatchingActions(ResourceLinkParser sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/ambiguousaction/{1}", host, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithoutMatchingAction(ResourceLinkParser sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/nogetaction/{1}", host, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithoutMatchingController(
            ResourceLinkParser sut, string host, int id, object controller)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/{1}/{2}", host, controller, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithoutMatchingRoute(ResourceLinkParser sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/foo/{1}", host, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void SutIsActionVerifier(ResourceLinkParser sut)
        {
            Assert.IsAssignableFrom<IActionVerifier>(sut);
        }

        [Theory]
        [AutoHypData]
        public void SutIsResourceLinkParser(ResourceLinkParser sut)
        {
            Assert.IsAssignableFrom<IResourceLinkParser>(sut);
        }

        [Theory]
        [AutoHypData]
        public void TryParseUriReturnsFalseOnUriWithMultipleMatchingActions(
            ResourceLinkParser sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/ambiguousaction/{1}", host, id));

            HttpActionContext dummy;
            var actual = sut.TryParse(uri, out dummy);

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void TryParseUriReturnsFalseOnUriWithoutMatchingAction(ResourceLinkParser sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/nogetaction/{1}", host, id));

            HttpActionContext dummy;
            var actual = sut.TryParse(uri, out dummy);

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void TryParseUriReturnsFalseOnUriWithoutMatchingController(
            ResourceLinkParser sut, string host, int id, string controller)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/{1}/{2}", host, controller, id));

            HttpActionContext dummy;
            var actual = sut.TryParse(uri, out dummy);

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void TryParseUriReturnsFalseOnUriWithoutMatchingRoute(ResourceLinkParser sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/foo/{1}", host, id));

            HttpActionContext dummy;
            var actual = sut.TryParse(uri, out dummy);

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void VerifyReturnsFalseWhenActionContextIsForCorrectControllerButWrongAction(ResourceLinkParser sut, int id)
        {
            var actionContext = GetActionContext<FooController>(x => x.GetById(id));

            var actual = sut.Verify<FooController>(actionContext, x => x.GetDefault());

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void VerifyReturnsFalseWhenActionContextIsForWrongController(ResourceLinkParser sut)
        {
            var actionContext = GetActionContext<FooController>(x => x.GetDefault());

            var actual = sut.Verify<BarController>(actionContext, x => x.GetDefault());

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void VerifyReturnsTrueWhenActionContextIsForMethodDeclaredOnControllerBaseType(ResourceLinkParser sut)
        {
            var actionContext = GetActionContext<DerivedController>(x => x.BaseMethod());

            var actual = sut.Verify<DerivedController>(actionContext, x => x.BaseMethod());

            Assert.True(actual);
        }

        [Theory]
        [AutoHypData]
        public void VerifyReturnsFalseWhenActionContextIsForMethodDeclaredOnControllerBaseTypeButActionControllerIsDifferentFromSpecifiedController(ResourceLinkParser sut)
        {
            var actionContext = GetActionContext<DerivedController>(x => x.BaseMethod());

            var actual = sut.Verify<DerivedController2>(actionContext, x => x.BaseMethod());

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void VerifyReturnsTrueWhenActionContextMatchesExpression(ResourceLinkParser sut, int id)
        {
            var actionContext = GetActionContext<FooController>(x => x.GetById(id));

            var actual = sut.Verify<FooController>(actionContext, x => x.GetById(default(int)));

            Assert.True(actual);
        }

        private static HttpActionContextResemblance GetActionContext<TController>(
            Expression<Action<TController>> expectedAction)
        {
            return new HttpActionContextResemblance(expectedAction.ToActionContext());
        }
    }
}
