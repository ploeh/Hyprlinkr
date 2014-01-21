using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using Ploeh.Hyprlinkr;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using System.Linq.Expressions;
using Ploeh.Hyprlinkr.UnitTest.Controllers;
using Moq;
using Ploeh.AutoFixture;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class ScalarRouteValuesQueryTest
    {
        [Theory, AutoHypData]
        public void SutHasAppropriateGuards(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ScalarRouteValuesQuery));
        }

        [Theory, AutoHypData]
        public void SutIsRouteValuesQuery(ScalarRouteValuesQuery sut)
        {
            Assert.IsAssignableFrom<IRouteValuesQuery>(sut);
        }

        [Theory, AutoHypData]
        public void GetRouteValuesForSingleParameterMethodReturnsCorrectResult(
            Mock<ScalarRouteValuesQuery> sutStub,
            int id,
            IDictionary<string, object> parameterValues)
        {
            Expression<Action<FooController>> exp = c => c.GetById(id);
            var methodCallExp = (MethodCallExpression)exp.Body;
            var pi = methodCallExp.Method.GetParameters().Single();
            sutStub
                .Setup(s => s.GetParameterValues(methodCallExp, pi))
                .Returns(parameterValues);

            IDictionary<string, object> actual =
                sutStub.Object.GetRouteValues(methodCallExp);

            var expected =
                new HashSet<KeyValuePair<string, object>>(parameterValues);
            Assert.True(expected.SetEquals(actual));
        }

        [Theory, AutoHypData]
        public void GetRouteValuesForParameterLessMethodReturnsCorrectResult(
            ScalarRouteValuesQuery sut)
        {
            Expression<Action<FooController>> exp = c => c.GetDefault();
            var methodCallExp = (MethodCallExpression)exp.Body;

            var actual = sut.GetRouteValues(methodCallExp);

            Assert.Empty(actual);
        }

        [Theory, AutoHypData]
        public void GetRouteValuesForTwoParameterMethodReturnsCorrectResult(
            Mock<ScalarRouteValuesQuery> sutStub,
            int id,
            string bar,
            Generator<IDictionary<string, object>> routeValueDictionaries)
        {
            // Fixture setup
            Expression<Action<BarController>> exp =
                c => c.GetWithIdAndQueryParameter(id, bar);
            var methodCallExp = (MethodCallExpression)exp.Body;
            var parameters = methodCallExp.Method.GetParameters();

            var expected = parameters.Zip(routeValueDictionaries, (p, d) =>
                {
                    sutStub
                        .Setup(s => s.GetParameterValues(methodCallExp, p))
                        .Returns(d);
                    return d;
                })
                .SelectMany(d => d)
                .ToHashSet();

            // Exercise system
            var actual = sutStub.Object.GetRouteValues(methodCallExp);

            // Verify outcome
            Assert.True(expected.SetEquals(actual));

            // Teardown
        }

        [Theory]
        [InlineAutoHypData(0)]
        [InlineAutoHypData(1)]
        public void GetParameterValueReturnsCorrectResult(
            int index,
            ScalarRouteValuesQuery sut,
            int id,
            string bar)
        {
            Expression<Action<BarController>> exp =
                c => c.GetWithIdAndQueryParameter(id, bar);
            var methodCallExp = (MethodCallExpression)exp.Body;
            var parameters = methodCallExp.Method.GetParameters();

            var actual =
                sut.GetParameterValues(methodCallExp, parameters[index]);

            var expected =
                new Dictionary<string, object> 
                    {
                        { "id", id.ToString() },
                        { "bar", bar } 
                    }
                    .ToArray()[index];
            Assert.Contains(expected, actual);
        }
    }
}
