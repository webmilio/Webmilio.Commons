using System;
using System.Collections.Generic;
using System.Reflection;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;
using Webmilio.Commons.Resolver;

namespace Webmilio.Commons.Resolvers;

public class TypeResolver<T> : IResolver<Type>
{
    public virtual IList<Type> Resolve(IList<Assembly> assemblies)
    {
        var resolved = new List<Type>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Concrete<T>();
            resolved.AddRange(types);

            foreach (var type in types)
            {
                Resolve(type);
            }
        }

        PostResolve(assemblies, resolved);
        return resolved;
    }


    protected virtual void Resolve(Type type) { }

    protected virtual void PostResolve(IList<Assembly> assemblies, List<Type> types) { }
}