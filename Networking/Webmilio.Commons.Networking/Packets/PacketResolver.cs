using System;
using System.Collections.Generic;
using Webmilio.Commons.DependencyInjection;

namespace Webmilio.Commons.Networking.Packets;

public class PacketResolver : IPacketResolver
{
    private readonly IServiceCollection _services;

    private short lastIndex = 1;
    private readonly Dictionary<short, Type> _types = new();
    private readonly Dictionary<Type, short> _ids = new();

    public PacketResolver(IServiceCollection services)
    {
        _services = services;
    }

    public void Map(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            Map(type);
        }
    }

    public void Map(Type type)
    {
        _types.Add(lastIndex, type);
        _ids.Add(type, lastIndex++);
    }

    public IPacket CreatePacket(short type)
    {
        return (IPacket) _services.Make(_types[type]);
    }

    public short GetTypeId(Type type)
    {
        return _ids[type];
    }
}