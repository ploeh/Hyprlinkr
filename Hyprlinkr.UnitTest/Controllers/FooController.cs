using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
