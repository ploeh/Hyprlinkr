using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ploeh.Hyprlinkr;

namespace Ploeh.Samples.Hyprlinkr.ExampleService
{
    public class HomeController : ApiController
    {
        private readonly RouteLinker linker;

        public HomeController(RouteLinker linker)
        {
            this.linker = linker;
        }

        public IEnumerable<HomeModel> Get()
        {
            yield return new HomeModel { Name = "ploeh" };
            yield return new HomeModel { Name = "fnaah" };
        }

        public HomeModel Get(string id)
        {
            return new HomeModel { Name = id };
        }
    }
}