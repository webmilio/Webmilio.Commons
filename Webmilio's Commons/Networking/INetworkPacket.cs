using System.IO;
using Webmilio.Commons.Networking.Resolver;

namespace Webmilio.Commons.Networking
{
    public interface INetworkPacket
    {
        void Send(INetworkPacketResolver resolver, BinaryWriter writer, object caller);

        void Receive(INetworkPacketResolver resolver, BinaryReader reader, object caller);


        [Ignore]
        public short Id { get; set; }
    }
}