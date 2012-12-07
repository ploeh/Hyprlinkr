using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public static class Enumerable
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}
