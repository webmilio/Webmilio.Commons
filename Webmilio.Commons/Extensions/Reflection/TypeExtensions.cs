using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Webmilio.Commons.Helpers;

namespace Webmilio.Commons.Extensions.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<TypeInfo> Concrete(this IEnumerable<TypeInfo> types) => types.Where(t => !t.IsAbstract && !t.IsInterface);

        public static IEnumerable<TypeInfo> Concrete<T>(this IEnumerable<TypeInfo> types)
        {
            TypeInfo type = typeof(T).GetTypeInfo();

            foreach (TypeInfo typeInfo in types.Concrete())
                if (type.IsAssignableFrom(typeInfo))
                    yield return typeInfo;

            // Concrete(types).Where(t => t.IsSubclassOf(typeof(T)));
        }


        public static string NamespaceAsPath(this Type type) => type.Namespace.Replace('.', '\\');

        public static string RootNamespace(this Type type) => type.Namespace.Split('.')[0];


        public static object Create(this Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }

        public static T Create<T>(this Type type, params object[] args)
        {
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"Type-generic {nameof(T)} must be the same type as or a child of {type}.");

            return (T)Create(type, args);
        }


        public static object Create(this TypeInfo type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }

        public static T Create<T>(this TypeInfo type, params object[] args)
        {
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"Type-generic {nameof(T)} must be the same type as or a child of {type}.");

            return (T) Create(type, args);
        }

        public static object[] Create(this IList<TypeInfo> types)
        {
            var objects = new object[types.Count];

            types.Do((t, i) => objects[i] = t.Create());
            return objects;
        }

        public static T[] Create<T>(this IList<TypeInfo> types)
        {
            var objects = new T[types.Count];

            types.Do((t, i) => objects[i] = t.Create<T>());
            return objects;
        }
    }
}