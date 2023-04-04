using System.IO;

namespace Webmilio.Commons.Networking.Packets;

public abstract class Packet : IPacket
{
    public virtual bool PreReceive(BinaryReader reader)
    {
        return true;
    }

    public virtual bool Receive(BinaryReader reader)
    {
        return true;
    }

    public virtual bool PreSend(BinaryWriter writer)
    {
        return true;
    }

    public virtual bool Send(BinaryWriter writer)
    {
        return true;
    }

    [Ignore] public short Identifier { get; set; }
}