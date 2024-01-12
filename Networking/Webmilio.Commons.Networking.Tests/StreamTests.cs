using System.Reflection;
using Webmilio.Commons.DependencyInjection;
using Webmilio.Commons.Networking.Packets;
using Webmilio.Commons.Resolvers;
using Webmilio.Commons.Serialization;

namespace Webmilio.Commons.Networking.Tests;

[TestClass]
public class StreamTests
{
    private OldServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new();
        _services
            .AddSingleton<NetworkHandler>()
            .AddSingleton<PacketMapper>()
            .AddSingleton<PacketResolver>()
            .AddSingleton<NetworkSerializers.Binary>()
            .AddSingleton<TypeResolver<Packet>>();

        var types = _services.GetService<TypeResolver<Packet>>().Resolve(new Assembly[]
        {
            GetType().Assembly
        });
        _services.GetService<PacketResolver>().Map(types);
    }

    [TestMethod]
    public void SimpleMemoryStream()
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        using var br = new BinaryReader(ms);

        const string Content = "test string";

        var network = _services.GetService<NetworkHandler>();
        network.Send(bw, new SimpleTestPacket()
        {
            Content1 = Content
        });

        ms.Seek(0, SeekOrigin.Begin);

        var packet = network.Receive(br) as SimpleTestPacket;
        Assert.AreEqual(Content, packet.Content1);
    }

    [TestMethod]
    public void ArrayMemoryStream()
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        using var br = new BinaryReader(ms);

        int[] Content = { 1, 2, 3, 4, 5 };

        var network = _services.GetService<NetworkHandler>();
        network.Send(bw, new ArrayTestPacket()
        {
            Content = Content
        });

        ms.Seek(0, SeekOrigin.Begin);

        var packet = network.Receive(br) as ArrayTestPacket;
        Assert.AreEqual(Content.Length, packet.Content.Length);

        for (int i = 0; i < Content.Length; i++)
        {
            Assert.AreEqual(Content[i], packet.Content[i]);
        }
    }

    [TestMethod]
    public void ComplexMemoryStream()
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        using var br = new BinaryReader(ms);

        const string Content1 = "test string";
        const int Content2 = 52;
        char[] Content3 = Content1.ToCharArray();

        var network = _services.GetService<NetworkHandler>();
        network.Send(bw, new ComplexTestPacket()
        {
            Content1 = Content1,
            Content2 = Content2,
            Content3 = Content3
        });

        ms.Seek(0, SeekOrigin.Begin);
        var packet = network.Receive(br) as ComplexTestPacket;

        Assert.AreEqual(Content1, packet.Content1);
        Assert.AreEqual(Content2, packet.Content2);

        Assert.AreEqual(Content3.Length, packet.Content3.Length);
        for (int i = 0; i < Content3.Length; i++)
        {
            Assert.AreEqual(Content3[i], packet.Content3[i]);
        }
    }

    private class SimpleTestPacket : Packet
    {
        public string Content1 { get; set; }
    }

    private class ArrayTestPacket : Packet
    {
        public int[] Content { get; set; }
    }

    private class ComplexTestPacket : Packet
    {
        public string Content1 { get; set; }
        public int Content2 { get; set; }
        public char[] Content3 { get; set; }
    }
}