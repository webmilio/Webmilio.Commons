using System;
using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.Extensions.Reflection
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<TypeInfo> Concrete(this IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                foreach (var type in assembly.Concrete())
                    yield return type;
        }

        public static IEnumerable<TypeInfo> Concrete<T>(this IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                foreach (var type in assembly.Concrete<T>())
                    yield return type;
        }

        public static IEnumerable<TypeInfo> Concrete(this Assembly assembly) => assembly.DefinedTypes.Concrete();

        public static IEnumerable<TypeInfo> Concrete<T>(this Assembly assembly) => assembly.DefinedTypes.Concrete<T>();
    }
}