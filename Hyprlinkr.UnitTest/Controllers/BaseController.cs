using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class BaseController : ApiController
    {
        public HttpResponseMessage BaseMethod()
        {
            return this.Request.CreateResponse();
        }
    }
}
