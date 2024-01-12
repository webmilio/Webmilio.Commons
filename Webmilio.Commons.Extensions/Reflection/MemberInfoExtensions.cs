using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.Extensions.Reflection;

public static class MemberInfoExtensions
{
    public static bool TryGetCustomAttribute<T>(this MemberInfo member, out T attribute) where T : Attribute
    {
        attribute = member.GetCustomAttribute<T>();
        return attribute != null;
    }

    public static Type[] GetTypes(this IList<FieldInfo> fields)
    {
        var types = new Type[fields.Count];

        for (int i = 0; i < fields.Count; i++)
        {
            types[i] = fields[i].FieldType;
        }

        return types;
    }

    public static Type[] GetTypes(this IList<PropertyInfo> properties)
    {
        var types = new Type[properties.Count];

        for (int i = 0; i < properties.Count; i++)
        {
            types[i] = properties[i].PropertyType;
        }

        return types;
    }

    public static Type[] GetTypes(this IList<ParameterInfo> parameters)
    {
        var types = new Type[parameters.Count];

        for (int i = 0; i < parameters.Count; i++)
        {
            types[i] = parameters[i].ParameterType;
        }

        return types;
    }

    public static bool HasCustomAttribute<T>(this MemberInfo member) where T : Attribute => TryGetCustomAttribute<T>(member, out _);
}