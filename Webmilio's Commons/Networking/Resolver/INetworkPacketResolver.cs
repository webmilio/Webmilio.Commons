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

        void Send(object caller, BinaryWriter writer, INetworkPacket packet);


        INetworkPacket Receive(object caller, BinaryReader reader);


        INetworkPacketMapper Mapper { get; }
    }
}