using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ploeh.Samples.Hyprlinkr.ExampleService
{
    [XmlType("link", Namespace = "http://www.w3.org/2005/Atom")]
    public class AtomLinkModel
    {
        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("rel")]
        public string Rel { get; set; }
    }
}