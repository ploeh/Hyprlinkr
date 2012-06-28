using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class HyprlinkrCustomization : CompositeCustomization
    {
        public HyprlinkrCustomization()
            : base(
                new HttpSchemeCustomization(),
                new AutoMoqCustomization(),
                new HttpRequestMessageCustomization()
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

        private class HttpRequestMessageCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var config = fixture.CreateAnonymous<HttpConfiguration>();

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
