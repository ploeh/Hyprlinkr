using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class FooController : ApiController
    {
        public object GetDefault()
        {
            return new object();
        }

        public object GetById(int id)
        {
            return new object();
        }

        public object GetWithIdAndOptionalParameter(int id, string bar, string foo = "foo")
        {
            return new object();
        }

        public object GetWithPloehAndFnaah(int ploeh, string fnaah)
        {
            return new object();
        }
    }

    public class FooAsyncController : ApiController
    {
        public Task<object> GetDefault()
        {
            return Task.Factory.StartNew(() => new object());
        }

        public Task<object> GetById(int id)
        {
            return Task.Factory.StartNew(() => new object());
        }

        public Task<object> GetWithIdAndOptionalParameter(int id, string bar, string foo = "foo")
        {
            return Task.Factory.StartNew(() => new object());
        }

        public Task<object> GetWithPloehAndFnaah(int ploeh, string fnaah)
        {
            return Task.Factory.StartNew(() => new object());
        }
    }
}
