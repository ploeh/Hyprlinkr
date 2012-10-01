using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class FooBarController : ApiController
    {
        public object GetBar(int id, int bar)
        {
            return new object();
        } 
    }
}