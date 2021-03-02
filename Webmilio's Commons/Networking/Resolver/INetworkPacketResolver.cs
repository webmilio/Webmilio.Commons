using System;
using System.IO;
using Webmilio.Commons.Networking.Resolver.Mapper;
using Webmilio.Commons.Resolvers;

namespace Webmilio.Commons.Networking.Resolver
{
    public interface INetworkPacketResolver : IResolver
    {
        short GetPacketId(Type type);

        void Send(BinaryWriter writer, INetworkPacket packet, object caller);


        INetworkPacket Receive(BinaryReader reader, object caller);


        INetworkPacketMapper Mapper { get; }
    }
}