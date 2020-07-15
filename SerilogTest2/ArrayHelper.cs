using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerilogTest2
{
    internal static class ArrayHelper
    {
        private static class EmptyArray<T>
        {
            internal static readonly T[] Instance = new T[0];
        }

        internal static T[] Empty<T>()
        {
            // TODO Use Array.Empty<T> in NET 4.6 when we are ready
            return EmptyArray<T>.Instance;
        }
    }
}
