using System;
using System.IO;
using System.Reflection;
using Webmilio.Commons.Networking.Resolver.Mapper;
using Webmilio.Commons.Resolvers;

namespace Webmilio.Commons.Networking.Resolver
{
    public interface INetworkPacketResolver : IResolver<TypeInfo>
    {
        short GetPacketId(Type type);

        void Send(BinaryWriter writer, INetworkPacket packet, object caller);


        INetworkPacket Receive(BinaryReader reader, object caller);


        INetworkPacketMapper Mapper { get; }
    }
}