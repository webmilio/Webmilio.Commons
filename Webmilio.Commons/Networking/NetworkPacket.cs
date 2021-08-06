using System.IO;
using Webmilio.Commons.Networking.Resolver;

namespace Webmilio.Commons.Networking
{
    public abstract class NetworkPacket : INetworkPacket
    {
        public void Send(INetworkPacketResolver resolver, BinaryWriter writer, object caller)
        {
            if (!PreSend(resolver, writer, caller))
                return;

            writer.Write(Id);
            resolver.Mapper.GetMapping(GetType()).Write(this, writer);

            PostSend(resolver, writer, caller);
        }

        protected virtual bool PreSend(INetworkPacketResolver resolver, BinaryWriter writer, object caller) => true;
        protected virtual void PostSend(INetworkPacketResolver resolver, BinaryWriter writer, object caller) { }


        public void Receive(INetworkPacketResolver resolver, BinaryReader reader, object caller)
        {
            if (!PreReceive(resolver, reader, caller))
                return;

            resolver.Mapper.GetMapping(GetType()).Read(this, reader);

            PostReceive(resolver, reader, caller);
        }

        protected virtual bool PreReceive(INetworkPacketResolver resolver, BinaryReader reader, object caller) => true;
        protected virtual void PostReceive(INetworkPacketResolver resolver, BinaryReader reader, object caller) { }


        [Ignore]
        public short Id { get; set; }
    }
}