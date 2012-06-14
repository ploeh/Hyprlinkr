using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class AutoHypDataAttribute : AutoDataAttribute
    {
        public AutoHypDataAttribute()
            : base(new Fixture().Customize(new HyprlinkrCustomization()))
        {
        }
    }
}
