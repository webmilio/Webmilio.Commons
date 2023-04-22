using System.Text;

namespace Webmilio.Commons.Serialization.Tests;

[TestClass]
public class SerializationTests
{
    private TestSerializers _ts = new();

    [TestMethod]
    public void Serialize()
    {
        const string Content = "test string";
        var serializer = _ts.Get(typeof(string));

        var serialized = serializer.Serialize(Content);
        var deserialized = (string) serializer.Deserialize(serialized);

        Assert.AreEqual(Content, deserialized);
    }

    private class TestSerializers : TypeBasedRegistry<TestSerializers.TestSerializer>
    {
        public TestSerializers()
        {
            Add(typeof(string), new TestSerializer.StringSerializer());
        }

        public abstract class TestSerializer : ISerializer<object, byte[]>
        {
            internal class StringSerializer : TestSerializer
            {
                public override object Deserialize(byte[] bytes)
                {
                    return Encoding.UTF8.GetString(bytes);
                }

                public override byte[] Serialize(object data)
                {
                    return Encoding.UTF8.GetBytes((string) data);
                }
            }

            public abstract byte[] Serialize(object input);
            public abstract object Deserialize(byte[] input);
        }

        public delegate object Deserialize(byte[] bytes);
        public delegate byte[] Serialize(object data);
    }
}