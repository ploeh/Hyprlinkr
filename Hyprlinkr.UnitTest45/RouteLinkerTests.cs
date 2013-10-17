using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.Hyprlinkr.UnitTest45
{
    public class RouteLinkerTests
    {
        [Theory, AutoHyp45Data]
        public void GetAsyncRouteForGetMethodWithIdReturnsCorrectResult(
            [Frozen]HttpRequestMessage request,
            RouteLinker sut,
            int id)
        {
            request.AddDefaultRoute();

            Uri actual = sut.GetUriAsync((AsyncController c) => c.Get(id)).Result;

            var baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority);
            var expected = new Uri(new Uri(baseUri), "api/async/" + id);
            Assert.Equal(expected, actual);
        }
    }
}
