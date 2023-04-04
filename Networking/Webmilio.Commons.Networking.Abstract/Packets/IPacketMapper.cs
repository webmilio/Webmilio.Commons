using System;
using Webmilio.Commons.Mappings;

namespace Webmilio.Commons.Networking.Packets;

public interface IPacketMapper<T>
{
    public TypeMembersMapping<T> Get(Type type);
}