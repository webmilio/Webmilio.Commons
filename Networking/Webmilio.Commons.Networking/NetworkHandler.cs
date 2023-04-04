using System.IO;
using Webmilio.Commons.Networking.Packets;
using Webmilio.Commons.Serialization;

namespace Webmilio.Commons.Networking;

public class NetworkHandler : INetworkHandler
{
    private readonly IPacketResolver _resolver;
    private readonly IPacketMapper<NetworkSerializers.Binary.Serializer> _mappings;

    public NetworkHandler(IPacketResolver resolver, IPacketMapper<NetworkSerializers.Binary.Serializer> mappings)
    {
        _resolver = resolver;
        _mappings = mappings;
    }

    public bool Send(BinaryWriter writer, IPacket packet)
    {
        packet.Identifier = _resolver.GetTypeId(packet.GetType());

        var mapping = _mappings.Get(packet.GetType());
        var wrappers = mapping.Wrappers;

        writer.Write(packet.Identifier);

        for (int i = 0; i < wrappers.Count; i++)
        {
            wrappers[i].mapping.Serialize(writer, wrappers[i].member.Get(packet));
        }

        return packet.Send(writer);
    }

    public IPacket Receive(BinaryReader reader)
    {
        var identifier = reader.ReadInt16();

        var packet = _resolver.CreatePacket(identifier);
        packet.Identifier = identifier;

        var mapping = _mappings.Get(packet.GetType());
        var wrappers = mapping.Wrappers;

        for (int i = 0; i < wrappers.Count; i++)
        {
            wrappers[i].member.Set(packet, wrappers[i].mapping.Deserialize(reader));
        }

        packet.Receive(reader);
        return packet;
    }
}