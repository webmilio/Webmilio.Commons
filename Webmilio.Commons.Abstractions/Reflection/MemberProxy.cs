using System;
using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.Reflection;

public class MemberProxy
{
    public delegate void SetMethod(object obj, object value);
    public delegate object GetMethod(object obj);

    public MemberProxy(MemberInfo member, Type type, SetMethod set, GetMethod get)
    {
        Member = member;
        Type = type;

        Set = set;
        Get = get;
    }

    public static MemberProxy ForField(FieldInfo field)
    {
        return new MemberProxy(field, field.FieldType,
            field.SetValue, field.GetValue);
    }

    public static MemberProxy ForProperty(PropertyInfo property)
    {
        return new MemberProxy(property, property.PropertyType,
            property.SetValue, property.GetValue);
    }

    public static IEnumerable<MemberProxy> GetMemberProxies(Type type, BindingFlags flags)
    {
        return GetMemberProxies(type, flags, flags);
    }

    public static IEnumerable<MemberProxy> GetMemberProxies(Type type, BindingFlags commonFlags, BindingFlags fieldFlags, BindingFlags propertyFlags)
    {
        return GetMemberProxies(type, commonFlags | fieldFlags, commonFlags | propertyFlags, _ => true);
    }

    public static IEnumerable<MemberProxy> GetMemberProxies(Type type, BindingFlags commonFlags, BindingFlags fieldFlags, BindingFlags propertyFlags, Predicate<MemberInfo> filter)
    {
        return GetMemberProxies(type, commonFlags | fieldFlags, commonFlags | propertyFlags, filter);
    }

    public static IEnumerable<MemberProxy> GetMemberProxies(Type type, BindingFlags fieldFlags, BindingFlags propertyFlags)
    {
        return GetMemberProxies(type, fieldFlags, propertyFlags, _ => true);
    }

    public static IEnumerable<MemberProxy> GetMemberProxies(Type type, BindingFlags fieldFlags, BindingFlags propertyFlags, Predicate<MemberInfo> filter)
    {
        foreach (var field in type.GetFields(fieldFlags))
            if (filter(field))
                yield return ForField(field);

        foreach (var property in type.GetProperties(propertyFlags))
            if (filter(property))
                yield return ForProperty(property);
    }

    public MemberInfo Member { get; }

    public SetMethod Set { get; }
    public GetMethod Get { get; }

    public Type Type { get; }
}