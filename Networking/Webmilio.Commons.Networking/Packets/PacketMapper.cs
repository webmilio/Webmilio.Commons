using System;
using System.Collections.Generic;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Mappings;
using Webmilio.Commons.Serialization;

namespace Webmilio.Commons.Networking.Packets;

using NSBS = NetworkSerializers.Binary.Serializer;

// TODO Make this a type-generic to allow for things other than Binary Serializers.
public class PacketMapper : IPacketMapper<NSBS>
{
    private readonly ITypeBasedRegistry<NSBS> _serializers;
    private readonly Dictionary<Type, TypeMembersMapping<NSBS>> _mappings = new();

    public PacketMapper(ITypeBasedRegistry<NSBS> serializers)
    {
        _serializers = serializers;
    }

    public TypeMembersMapping<NSBS> Get(Type type)
    {
        return _mappings.GetValueOrAdd(type, Map);
    }

    private TypeMembersMapping<NSBS> Map(Type type)
    {
        return new TypeMembersMapping<NSBS>(type, _serializers);
    }
}