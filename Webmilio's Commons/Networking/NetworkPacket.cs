using System.IO;
using Webmilio.Commons.Networking.Resolver;

namespace Webmilio.Commons.Networking
{
    public abstract class NetworkPacket : INetworkPacket
    {
        public void Send(INetworkPacketResolver resolver, BinaryWriter writer)
        {
            if (!PreSend(resolver, writer))
                return;

            writer.Write(Id);
            resolver.Mapper.GetMapping(GetType()).Write(this, writer);

            PostSend(resolver, writer);
        }

        protected bool PreSend(INetworkPacketResolver resolver, BinaryWriter writer) => true;
        protected void PostSend(INetworkPacketResolver resolver, BinaryWriter writer) { }


        public void Receive(INetworkPacketResolver resolver, BinaryReader reader)
        {
            if (!PreReceive(resolver, reader))
                return;

            resolver.Mapper.GetMapping(GetType()).Read(this, reader);

            PostReceive(resolver, reader);
        }

        protected bool PreReceive(INetworkPacketResolver resolver, BinaryReader reader) => true;
        protected void PostReceive(INetworkPacketResolver resolver, BinaryReader reader) { }


        [Ignore]
        public short Id { get; set; }
    }
}