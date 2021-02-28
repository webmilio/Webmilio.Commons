using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;
using Webmilio.Commons.Networking.Resolver.Mapper;

namespace Webmilio.Commons.Networking.Resolver
{
    public class NetworkPacketResolver : INetworkPacketResolver
    {
        public const short StartingPacketId = 1;

        private short _currentId = StartingPacketId;

        private readonly Dictionary<Type, short> _toId = new Dictionary<Type, short>();
        private readonly Dictionary<short, Type> _toType = new Dictionary<short, Type>();


        public NetworkPacketResolver()
        {
            Mapper = new NetworkPacketMapper();
            Resolve(AppDomain.CurrentDomain.GetAssemblies());
        }


        public void Resolve(params Assembly[] assemblies)
        {
            assemblies.Do(delegate (Assembly assembly)
            {
                assembly.DefinedTypes.Concrete<INetworkPacket>().Do(delegate (TypeInfo type)
                {
                    _toId.Add(type, _currentId);
                    _toType.Add(_currentId++, type);

                    Mapper.Map(type);
                });
            });
        }

        public short GetPacketId<T>() where T : INetworkPacket => GetPacketId(typeof(T));

        public short GetPacketId(Type type)
        {
            return _toId[type];
        }

        public void Send<T>(BinaryWriter writer) where T : INetworkPacket, new()
        {
            Send(writer, Activator.CreateInstance<T>());
        }

        public void Send(BinaryWriter writer, INetworkPacket packet)
        {
            if (!PreSend(writer))
                return;

            if (packet.Id == 0)
                packet.Id = _toId[packet.GetType()];

            packet.Send(this, writer);
            PostSend(writer, packet.Id, packet);

            writer.Flush();
        }

        public INetworkPacket Receive(BinaryReader reader)
        {
            if (!PreReceive(reader))
                return default;

            short packetId = reader.ReadInt16();

            if (packetId <= 0)
            {
                NonMappedPacketIdReceive(reader, packetId);
                return default;
            }

            var packet = (INetworkPacket)Activator.CreateInstance(_toType[packetId]);
            packet.Id = packetId;

            packet.Receive(this, reader);
            PostReceive(reader, packetId, packet);

            return packet;
        }


        protected virtual bool PreSend(BinaryWriter writer) => true;
        protected virtual void PostSend(BinaryWriter writer, short packetId, INetworkPacket packet) { }

        protected virtual bool PreReceive(BinaryReader reader) => true;
        protected virtual void PostReceive(BinaryReader reader, short packetId, INetworkPacket packet) { }
        protected virtual void NonMappedPacketIdReceive(BinaryReader reader, short packetId) { }


        public INetworkPacketMapper Mapper { get; }
    }
}