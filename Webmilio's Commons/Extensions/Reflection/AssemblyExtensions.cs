using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.Extensions.Reflection
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<TypeInfo> Concrete(this Assembly assembly) => assembly.DefinedTypes.Concrete();

        public static IEnumerable<TypeInfo> Concrete<T>(this Assembly assembly) => assembly.DefinedTypes.Concrete<T>();
    }
}