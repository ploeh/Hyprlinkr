using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ploeh.Hyprlinkr.UnitTest.Controllers
{
    public class SomeModel
    {
        public int Number { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return "number=" + this.Number + "&text=" + this.Text;
        }
    }
}
