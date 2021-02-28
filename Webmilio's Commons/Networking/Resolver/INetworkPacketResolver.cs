using System;
using System.IO;
using System.Reflection;
using Webmilio.Commons.Networking.Resolver.Mapper;

namespace Webmilio.Commons.Networking.Resolver
{
    public interface INetworkPacketResolver
    {
        void Resolve(Assembly[] assemblies);

        short GetPacketId(Type type);

        void Send(BinaryWriter writer, INetworkPacket packet);


        INetworkPacket Receive(BinaryReader reader);


        INetworkPacketMapper Mapper { get; }
    }
}