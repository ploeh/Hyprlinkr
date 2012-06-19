using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class BarController : ApiController
    {
        public object GetDefault()
        {
            return new object();
        }
    }
}
