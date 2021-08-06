using System;

namespace Webmilio.Commons.Networking.Resolver.Mapper
{
    public interface INetworkPacketMapper
    {
        void Map(Type type);

        PacketMapping GetMapping(Type type);
    }
}