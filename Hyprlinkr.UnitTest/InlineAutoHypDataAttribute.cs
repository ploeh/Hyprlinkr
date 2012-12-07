using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture.Xunit;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class InlineAutoHypDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoHypDataAttribute(params object[] values)
            : base(new AutoHypDataAttribute(), values)
        {
        }
    }
}
