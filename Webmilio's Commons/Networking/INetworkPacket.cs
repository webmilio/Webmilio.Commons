using System.IO;
using Webmilio.Commons.Networking.Resolver;

namespace Webmilio.Commons.Networking
{
    public interface INetworkPacket
    {
        void Send(INetworkPacketResolver resolver, BinaryWriter writer);

        void Receive(INetworkPacketResolver resolver, BinaryReader reader);


        [Ignore]
        public short Id { get; set; }
    }
}