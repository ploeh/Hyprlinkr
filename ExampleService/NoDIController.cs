using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ploeh.Hyprlinkr;

namespace Ploeh.Samples.Hyprlinkr.ExampleService
{
    public class NoDIController : ApiController
    {
        public NoDIModel Get(string id)
        {
            var linker = new RouteLinker(this.Request);
            return new NoDIModel
            {
                Links = new[]
                {
                    new AtomLinkModel
                    {
                        Href = linker.GetUri<NoDIController>(r => r.Get(id)).ToString(),
                        Rel = "self"
                    },
                    new AtomLinkModel
                    {
                        Href = linker.GetUri<HomeController>(r => r.Get()).ToString(),
                        Rel = "http://sample.ploeh.dk/rels/home"
                    }
                },
                Name = id
            };
        }
    }
}