using System;
using System.Collections.Generic;

namespace Webmilio.Commons.Networking.Resolver.Mapper
{
    public class NetworkPacketMapper : INetworkPacketMapper
    {
        private readonly Dictionary<Type, PacketMapping> _mappings = new Dictionary<Type, PacketMapping>();


        public void Map(Type type)
        {
            if (!typeof(INetworkPacket).IsAssignableFrom(type))
                throw new ArgumentException($"{nameof(NetworkPacketMapper)} can only be used to map children of {nameof(INetworkPacket)}.");

            _mappings.Add(type, new PacketMapping(type));
        }

        public PacketMapping GetMapping(Type type)
        {
            return _mappings[type];
        }
    }
}