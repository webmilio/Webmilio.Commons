using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Webmilio.Commons.Extensions.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<TypeInfo> Concrete(this IEnumerable<TypeInfo> types) => types.Where(t => !t.IsAbstract && !t.IsInterface);

        public static IEnumerable<TypeInfo> Concrete<T>(this IEnumerable<TypeInfo> types)
        {
            TypeInfo type = typeof(T).GetTypeInfo();

            foreach (TypeInfo typeInfo in types)
                if (type.IsAssignableFrom(typeInfo))
                    yield return typeInfo;

            // Concrete(types).Where(t => t.IsSubclassOf(typeof(T)));
        }


        public static string NamespaceAsPath(this Type type) => type.Namespace.Replace('.', '\\');

        public static string RootNamespace(this Type type) => type.Namespace.Split('.')[0];
    }
}