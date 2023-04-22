using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Webmilio.Commons.Extensions.Reflection;
using Webmilio.Commons.Reflection;

namespace Webmilio.Commons.Mappings;

public class TypeMembersMapping<T>
{
    public TypeMembersMapping(Type type, ITypeBasedRegistry<T> serializers)
    {
        Type = type;

        const BindingFlags FieldFlags = BindingFlags.SetField | BindingFlags.GetField;
        const BindingFlags PropertyFlags = BindingFlags.SetProperty | BindingFlags.GetProperty;

        Predicate<MemberInfo> filter = (memberInfo) => !memberInfo.HasCustomAttribute<CompilerGeneratedAttribute>();

        var @public = MemberProxy.GetMemberProxies(type, BindingFlags.Public | BindingFlags.Instance, FieldFlags, PropertyFlags, filter);
        var @nonPublic = MemberProxy.GetMemberProxies(type, BindingFlags.NonPublic | BindingFlags.Instance, FieldFlags, PropertyFlags, filter);

        var members = new List<MemberProxy>();
        var wrappers = new List<ProxyWrapper>();

        members.AddRange(@public);
        members.AddRange(@nonPublic);

        foreach (var member in members) 
        {
            if (!member.Member.HasCustomAttribute<IgnoreAttribute>() &&
                serializers.Has(member.Type))
            {
                wrappers.Add(new(member, serializers.Get(member.Type)));
            }
        }

        Wrappers = wrappers.AsReadOnly();
    }

    public Type Type { get; }

    public ReadOnlyCollection<ProxyWrapper> Wrappers { get; }

    public struct ProxyWrapper
    {
        public MemberProxy member;
        public T mapping;

        public ProxyWrapper(MemberProxy member, T mapping)
        {
            this.member = member;
            this.mapping = mapping;
        }
    }
}