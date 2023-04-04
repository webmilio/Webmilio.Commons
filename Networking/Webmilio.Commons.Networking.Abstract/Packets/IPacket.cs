using System.IO;

namespace Webmilio.Commons.Networking.Packets;

public interface IPacket
{
    public bool Receive(BinaryReader reader);

    public bool Send(BinaryWriter writer);

    [Ignore] public short Identifier { get; set; }
}