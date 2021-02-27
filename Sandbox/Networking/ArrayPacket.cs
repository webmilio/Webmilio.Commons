using Webmilio.Commons.Networking;

namespace WebmilioCommons.Sandbox.Networking
{
    public class ArrayPacket : NetworkPacket
    {
        public byte[] Numbers { get; set; }
    }
}