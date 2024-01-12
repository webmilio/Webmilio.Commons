using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Webmilio.Commons.Extensions.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> Concrete(this IEnumerable<Type> types) => types.Where(t => !t.IsAbstract && !t.IsInterface);

        public static IEnumerable<Type> Concrete<T>(this IEnumerable<Type> types)
        {
            foreach (var type in types.Concrete())
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
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

        public static IEnumerable<object> Create(this IEnumerable<TypeInfo> types)
        {
            foreach (var type in types)
                yield return type.Create();
        }

        public static IEnumerable<T> Create<T>(this IEnumerable<TypeInfo> types)
        {
            foreach (var type in types)
                yield return type.Create<T>();
        }
    }
}