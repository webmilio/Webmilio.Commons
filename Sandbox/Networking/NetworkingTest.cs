using System.IO;
using Webmilio.Commons.Networking.Resolver;

namespace WebmilioCommons.Sandbox.Networking
{
    public class NetworkingTest
    {
        private INetworkPacketResolver _resolver;


        public NetworkingTest(INetworkPacketResolver resolver)
        {
            _resolver = resolver;
        }


        public void Run()
        {
            var writer = new FakeBinaryWriter();
            var packet = new LoggedInPacket(45);

            _resolver.Send(writer, packet);

            var s = new MemoryStream();
            var bw = new BinaryWriter(s);

            bw.Write(packet.Id);
            bw.Write(packet.ClientId);
            bw.Flush();
            bw.BaseStream.Position = 0;

            _resolver.Receive(new BinaryReader(s));


            s = new MemoryStream();
            bw = new BinaryWriter(s);

            bw.Write(_resolver.GetPacketId(typeof(ArrayPacket)));
            bw.Write(5);

            for (int i = 0; i < 5; i++)
                bw.Write((byte) i);

            bw.Flush();
            bw.BaseStream.Position = 0;

            _resolver.Receive(new BinaryReader(s));
        }
    }
}