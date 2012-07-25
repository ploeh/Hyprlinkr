using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ploeh.Samples.Hyprlinkr.ExampleService
{
    [XmlRoot("home-root", Namespace = "http://www.ploeh.dk/hyprlinkr/sample/2012")]
    public class HomeModelCollection
    {
        [XmlArray("links")]
        [XmlArrayItem("link", Namespace = "http://www.w3.org/2005/Atom")]
        public AtomLinkModel[] Links { get; set; }

        [XmlArray("homes")]
        [XmlArrayItem("home")]
        public HomeModel[] Homes { get; set; }
    }
}