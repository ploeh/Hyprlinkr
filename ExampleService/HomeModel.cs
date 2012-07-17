using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ploeh.Samples.Hyprlinkr.ExampleService
{
    [XmlRoot("home", Namespace = "http://www.ploeh.dk/hyprlinkr/sample/2012")]
    public class HomeModel
    {
        [XmlArray("links")]
        [XmlArrayItem("link", Namespace = "http://www.w3.org/2005/Atom")]
        public AtomLinkModel[] Links { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }
    }
}