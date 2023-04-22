using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Webmilio.Commons.DependencyInjection;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;
using Webmilio.Commons.Helpers;

namespace Webmilio.Commons.Mapping;

public class Mapper
{
    private readonly Dictionary<Type, PropertyInfo[]> _properties = new();

    public T Map<T>(object origin)
    {
        var target = TypeHelpers.Instantiate<T>();
        return Map(origin, target);
    }

    public T Map<T>(object origin, IServiceContainer services)
    {
        var target = services.Make<T>();
        return Map(origin, target);
    }

    public T[] Map<T>(IList<object> elements)
    {
        var mapped = new T[elements.Count];

        elements.Do((e, i) => mapped[i] = Map<T>(e));
        return mapped;
    }

    public T[] Map<T>(IList<object> elements, IServiceContainer services)
    {
        var mapped = new T[elements.Count];

        elements.Do((e, i) => mapped[i] = Map<T>(e, services));
        return mapped;
    }

    private T Map<T>(object origin, T target)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | 
                                   BindingFlags.SetProperty | BindingFlags.GetProperty;

        var originProperties = _properties.GetValueOrAdd(origin.GetType(), t => t.GetProperties(flags));
        var targetProperties = _properties.GetValueOrAdd(typeof(T), t => t.GetProperties(flags));

        foreach (var tp in targetProperties)
        {
            if (tp.TryGetCustomAttribute<IgnoreAttribute>(out _)) continue;

            string tpName = tp.Name;
            if (tp.TryGetCustomAttribute(out NameAttribute attr)) tpName = attr.Name;

            foreach (var op in originProperties)
            {
                if (op.TryGetCustomAttribute<IgnoreAttribute>(out _)) continue;

                string opName = op.Name;
                if (op.TryGetCustomAttribute(out attr)) opName = attr.Name;

                if (tpName != opName || tp.PropertyType != op.PropertyType)
                    continue;

                var value = op.GetValue(origin);

                if (value != default)
                    tp.SetValue(target, value);
            }
        }

        return target;
    }

    public static Mapper Common { get; } = new();
}