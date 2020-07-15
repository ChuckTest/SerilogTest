using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SerilogTest2
{
    static class ReflectionHelpers
    {
        public static Assembly GetAssembly(this Type type)
        {
#if NETSTANDARD1_0
            var typeInfo = type.GetTypeInfo();
            return typeInfo.Assembly;
#else
            return type.Assembly;
#endif
        }
    }
}
