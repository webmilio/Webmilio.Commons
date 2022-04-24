using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Webmilio.Commons.DependencyInjection;
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

    private T Map<T>(object origin, T target)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | 
                                   BindingFlags.SetProperty | BindingFlags.GetProperty;

        _properties.GetValueOrDefault()

        var originProperties = origin.GetType().GetProperties(flags);
        var targetProperties = typeof(T).GetProperties(flags);

        foreach (var tp in targetProperties)
        {
            foreach (var op in originProperties)
            {
                if (tp.Name != op.Name || tp.PropertyType != op.PropertyType)
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