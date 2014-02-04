using System.Threading.Tasks;
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

    public class FooBarAsyncController : ApiController
    {
        public Task<object> GetBar(int id, int bar)
        {
            return Task.Factory.StartNew(() => new object());
        }
    }
}