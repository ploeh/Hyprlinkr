using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace Ploeh.Hyprlinkr.UnitTest45
{
    public class AutoHyp45DataAttribute : AutoDataAttribute
    {
        public AutoHyp45DataAttribute()
            : base(new Fixture().Customize(new Hyprlinkr45Customization()))
        {
        }
    }
}
