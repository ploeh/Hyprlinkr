using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Hosting;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Ploeh.Hyprlinkr.UnitTest45
{
    public class Hyprlinkr45Customization : CompositeCustomization
    {
        public Hyprlinkr45Customization()
            : base(
                new HttpSchemeCustomization(),
                new AutoMoqCustomization(),
                new HttpRequestMessageCustomization())
        {
        }

        private class HttpSchemeCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Inject(new UriScheme("http"));
            }
        }

        private class HttpRequestMessageCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var config = fixture.Create<HttpConfiguration>();

                fixture.Customize<HttpRequestMessage>(c => c
                    .Do(x =>
                    {
                        x.Properties[HttpPropertyKeys.HttpConfigurationKey] =
                            config;
                    }));
            }
        }
    }
}
