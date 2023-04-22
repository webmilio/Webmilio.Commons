using System.IO;
using Webmilio.Commons.Networking.Packets;

namespace Webmilio.Commons.Networking;

public interface INetworkHandler
{
    public bool Send(BinaryWriter writer, IPacket packet);
    public IPacket Receive(BinaryReader reader);
}