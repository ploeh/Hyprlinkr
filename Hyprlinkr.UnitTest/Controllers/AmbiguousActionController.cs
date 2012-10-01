using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class AmbiguousActionController : ApiController
    {
        public object GetById(int id)
        {
            return new object();
        }

        public object GetByIdAndParameter(int id, string foo = "foo")
        {
            return new object();
        }
    }
}
