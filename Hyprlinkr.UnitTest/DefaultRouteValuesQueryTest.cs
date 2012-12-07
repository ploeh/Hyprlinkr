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

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class DefaultRouteValuesQueryTest
    {
        [Theory, AutoHypData]
        public void SutHasAppropriateGuards(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(DefaultRouteValuesQuery));
        }

        [Theory, AutoHypData]
        public void SutIsRouteValuesQuery(DefaultRouteValuesQuery sut)
        {
            Assert.IsAssignableFrom<IRouteValuesQuery>(sut);
        }

        [Theory, AutoHypData]
        public void GetRouteValuesForSingleParameterMethodReturnsCorrectResult(
            Mock<DefaultRouteValuesQuery> sutStub,
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
    }
}
