using System;

namespace Webmilio.Commons.Networking.Packets;

public interface IPacketResolver
{
    public void Map(Type type);

    public IPacket CreatePacket(short type);

    public short GetTypeId(Type type);
}