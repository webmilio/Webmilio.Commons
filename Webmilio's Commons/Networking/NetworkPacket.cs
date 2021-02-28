using System.IO;
using Webmilio.Commons.Networking.Resolver;

namespace Webmilio.Commons.Networking
{
    public abstract class NetworkPacket : INetworkPacket
    {
        public void Send(INetworkPacketResolver resolver, object caller, BinaryWriter writer)
        {
            if (!PreSend(resolver, caller, writer))
                return;

            writer.Write(Id);
            resolver.Mapper.GetMapping(GetType()).Write(this, writer);

            PostSend(resolver, caller, writer);
        }

        protected virtual bool PreSend(INetworkPacketResolver resolver, object instance, BinaryWriter writer) => true;
        protected virtual void PostSend(INetworkPacketResolver resolver, object instance, BinaryWriter writer) { }


        public void Receive(INetworkPacketResolver resolver, object caller, BinaryReader reader)
        {
            if (!PreReceive(resolver, caller, reader))
                return;

            resolver.Mapper.GetMapping(GetType()).Read(this, reader);

            PostReceive(resolver, caller, reader);
        }

        protected virtual bool PreReceive(INetworkPacketResolver resolver, object caller, BinaryReader reader) => true;
        protected virtual void PostReceive(INetworkPacketResolver resolver, object caller, BinaryReader reader) { }


        [Ignore]
        public short Id { get; set; }
    }
}