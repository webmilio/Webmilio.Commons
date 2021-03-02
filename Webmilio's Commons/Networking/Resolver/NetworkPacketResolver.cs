using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Webmilio.Commons.Networking.Resolver.Mapper;
using Webmilio.Commons.Resolvers;

namespace Webmilio.Commons.Networking.Resolver
{
    public class NetworkPacketResolver : Resolver<INetworkPacket>, INetworkPacketResolver
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


        protected override void Resolve(TypeInfo type)
        {
            _toId.Add(type, _currentId);
            _toType.Add(_currentId++, type);

            Mapper.Map(type);
        }


        public short GetPacketId<T>() where T : INetworkPacket => GetPacketId(typeof(T));

        public short GetPacketId(Type type)
        {
            return _toId[type];
        }

        public void Send<T>(BinaryWriter writer, object caller) where T : INetworkPacket, new()
        {
            Send(writer, Activator.CreateInstance<T>(), caller);
        }

        public void Send(BinaryWriter writer, INetworkPacket packet, object caller)
        {
            if (!PreSend(writer, caller))
                return;

            if (packet.Id == 0)
                packet.Id = _toId[packet.GetType()];

            packet.Send(this, writer, caller);
            PostSend(writer, packet.Id, packet, caller);

            writer.Flush();
        }

        public INetworkPacket Receive(BinaryReader reader, object caller)
        {
            if (!PreReceive(reader, caller))
                return default;

            short packetId = reader.ReadInt16();

            if (packetId <= 0)
            {
                NonMappedPacketIdReceive(reader, packetId, caller);
                return default;
            }

            var packet = (INetworkPacket)Activator.CreateInstance(_toType[packetId]);
            packet.Id = packetId;

            packet.Receive(this, reader, caller);
            PostReceive(reader, packetId, packet, caller);

            return packet;
        }


        protected virtual bool PreSend(BinaryWriter writer, object caller) => true;
        protected virtual void PostSend(BinaryWriter writer, short packetId, INetworkPacket packet, object caller) { }

        protected virtual bool PreReceive(BinaryReader reader, object caller) => true;
        protected virtual void PostReceive(BinaryReader reader, short packetId, INetworkPacket packet, object caller) { }
        protected virtual void NonMappedPacketIdReceive(BinaryReader reader, short packetId, object caller) { }


        public INetworkPacketMapper Mapper { get; }
    }
}