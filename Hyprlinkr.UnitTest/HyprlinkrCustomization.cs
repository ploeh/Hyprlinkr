using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class HyprlinkrCustomization : CompositeCustomization
    {
        public HyprlinkrCustomization()
            : base(
                new HttpSchemeCustomization(),
                new AutoMoqCustomization()
            )
        {
        }

        private class HttpSchemeCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Inject(new UriScheme("http"));
            }
        }
    }
}
