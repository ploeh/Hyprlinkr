using System;
using System.Linq.Expressions;
using System.Web.Http.Controllers;
using Ploeh.AutoFixture.Idioms;
using Ploeh.Hyprlinkr.UnitTest.Controllers;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class ResourceLinkVerifierTests
    {
        [Theory]
        [AutoHypData]
        public void ConstructorHasAppropriateGuards(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ResourceLinkVerifier).GetConstructors());
        }

        [Theory]
        [AutoHypData]
        public void ParseUriReturnsCorrectValueForComplexRoute(ResourceLinkVerifier sut, string host, int id, int bar)
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
            ResourceLinkVerifier sut, string host, int id, string bar)
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
            ResourceLinkVerifier sut, string host, int id, string bar, string foo)
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
            ResourceLinkVerifier sut, string host, int id, string bar)
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
            ResourceLinkVerifier sut, string host, int id, string bar, string foo)
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
            ResourceLinkVerifier sut, string host, int id, string bar, string foo)
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
            ResourceLinkVerifier sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/foo/{1}", host, id));

            var actual = sut.Parse(uri);
            var expected = GetActionContext<FooController>(x => x.GetById(id));

            Assert.Equal(expected, actual, HttpActionContextResemblance.EqualityComparer);
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithMultipleMatchingActions(ResourceLinkVerifier sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/ambiguousaction/{1}", host, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithoutMatchingAction(ResourceLinkVerifier sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/nogetaction/{1}", host, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithoutMatchingController(
            ResourceLinkVerifier sut, string host, int id, object controller)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/{1}/{2}", host, controller, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void ParseUriThrowsOnUriWithoutMatchingRoute(ResourceLinkVerifier sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/foo/{1}", host, id));

            Assert.Throws<ArgumentException>(() => sut.Parse(uri));
        }

        [Theory]
        [AutoHypData]
        public void SutIsActionVerifier(ResourceLinkVerifier sut)
        {
            Assert.IsAssignableFrom<IActionVerifier>(sut);
        }

        [Theory]
        [AutoHypData]
        public void SutIsResourceLinkParser(ResourceLinkVerifier sut)
        {
            Assert.IsAssignableFrom<IResourceLinkParser>(sut);
        }

        [Theory]
        [AutoHypData]
        public void TryParseUriReturnsFalseOnUriWithMultipleMatchingActions(
            ResourceLinkVerifier sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/ambiguousaction/{1}", host, id));

            HttpActionContext dummy;
            var actual = sut.TryParse(uri, out dummy);

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void TryParseUriReturnsFalseOnUriWithoutMatchingAction(ResourceLinkVerifier sut, string host, int id)
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
            ResourceLinkVerifier sut, string host, int id, string controller)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/api/{1}/{2}", host, controller, id));

            HttpActionContext dummy;
            var actual = sut.TryParse(uri, out dummy);

            Assert.False(actual);
        }

        [Theory]
        [AutoHypData]
        public void TryParseUriReturnsFalseOnUriWithoutMatchingRoute(ResourceLinkVerifier sut, string host, int id)
        {
            sut.Configuration.AddDefaultRoute();
            var uri = new Uri(string.Format("http://{0}/foo/{1}", host, id));

            HttpActionContext dummy;
            var actual = sut.TryParse(uri, out dummy);

            Assert.False(actual);
        }

        [Theory, AutoHypData]
        public void VerifyReturnsTrueWhenActionContextMatchesExpression(ResourceLinkVerifier sut, int id)
        {
            var actionContext = GetActionContext<FooController>(x => x.GetById(id));

            var actual = sut.Verify<FooController>(actionContext, x => x.GetById(Arg.OfType<int>()));

            Assert.True(actual);
        }

        [Theory, AutoHypData]
        public void VerifyReturnsFalseWhenActionContextIsForCorrectControllerButWrongAction(ResourceLinkVerifier sut, int id)
        {
            var actionContext = GetActionContext<FooController>(x => x.GetById(id));

            var actual = sut.Verify<FooController>(actionContext, x => x.GetDefault());

            Assert.False(actual);
        }

        [Theory, AutoHypData]
        public void VerifyReturnsFalseWhenActionContextIsForWrongController(ResourceLinkVerifier sut)
        {
            var actionContext = GetActionContext<FooController>(x => x.GetDefault());

            var actual = sut.Verify<BarController>(actionContext, x => x.GetDefault());

            Assert.False(actual);
        }

        private static HttpActionContextResemblance GetActionContext<TController>(
            Expression<Action<TController>> expectedAction)
        {
            return new HttpActionContextResemblance(expectedAction.ToActionContext());
        }
    }
}
