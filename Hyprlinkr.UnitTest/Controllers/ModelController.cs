using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class ModelController : ApiController
    {
        public object Get(SomeModel queryModel)
        {
            return new object();
        }
    }

    public class ModelAsyncController : ApiController
    {
        public Task<object> Get(SomeModel queryModel)
        {
            return Task.Factory.StartNew(() => new object());
        }
    }
}
