using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLOChallenge.Helpers
{
    public static class ExtraLinq
    {
        public static void ForEachWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> Action)
        {
            int i = 0;
            foreach(var item in enumerable)
            {
                Action(item, i);
                i += 1;
            }
        }
    }
}
