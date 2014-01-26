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
using Moq;
using System.Reflection;
using Ploeh.AutoFixture.Kernel;
using System.Linq.Expressions;
using Ploeh.Hyprlinkr.UnitTest.Controllers;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class HyprlinkrCustomization : CompositeCustomization
    {
        public HyprlinkrCustomization()
            : base(
                new HttpSchemeCustomization(),
                new MethodCustomization(),
                new ParameterCustomization(),
                new MultipleCustomization(),
                new AutoMoqCustomization(),
                new HttpRequestMessageCustomization(),
                new HttpConfigurationCustomization()
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

        private class HttpConfigurationCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<HttpConfiguration>(c => c.Without(x => x.DependencyResolver));
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

        private class MethodCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<Mock<MethodInfo>>(c => c
                    .Do(stub => stub
                        .SetupGet(m => m.ReflectedType.Name)
                        .Returns(fixture.Create<string>())));

                Expression<Action<FooController>> exp = c => c.GetDefault();
                fixture.Inject((MethodCallExpression)exp.Body);
            }
        }

        private class ParameterCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                Expression<Action<FooController>> exp = c => c.GetById(0);
                fixture.Inject(((MethodCallExpression)exp.Body).Method.GetParameters().Single());
            }
        }
    }
}
