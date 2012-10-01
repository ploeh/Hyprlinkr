using System;
using System.Net.Http;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class NoGetActionController : ApiController
    {
        public HttpResponseMessage Post(string data)
        {
            throw new NotImplementedException();
        }
    }
}
