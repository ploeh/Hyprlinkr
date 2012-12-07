using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using Ploeh.Hyprlinkr;
using Ploeh.AutoFixture.Idioms;
using Xunit;

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
    }
}
