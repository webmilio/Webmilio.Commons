using Webmilio.Commons.Networking;

namespace WebmilioCommons.Sandbox.Networking
{
    public class LoggedInPacket : NetworkPacket
    {
        public LoggedInPacket()
        {
        }

        public LoggedInPacket(byte clientId)
        {
            ClientId = clientId;
        }


        public byte ClientId { get; set; }
    }
}