using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Ploeh.Hyprlinkr.UnitTest45
{
    public class AsyncController : ApiController
    {
        public async Task<object> Get(int id)
        {
            return await Task.Factory.StartNew(() => new object());
        }
    }
}
