using System;
using System.IO;

namespace Webmilio.Commons.Serialization;

public class NetworkSerializers
{
    public class Binary : TypeBasedRegistry<Binary.Serializer>
    {
        public class Serializer : StreamSerializer<BinaryReader, BinaryWriter, object>
        {
            public Serializer(Deserializer reader, Serializer writer) : base(reader, writer) { }
        }

        public override Serializer Get(Type type)
        {
            if (type.IsArray && !base.Has(type) && Has(type.GetElementType()))
            {
                var localType = type.GetElementType();

                var integerSerializer = Get<int>();
                var serializer = Get(localType);

                object Deserializer(BinaryReader reader)
                {
                    var count = (int) integerSerializer.Deserialize(reader);
                    var array = Array.CreateInstance(localType, count);

                    for (int i = 0; i < count; i++)
                    {
                        array.SetValue(serializer.Deserialize(reader), i);
                    }

                    return array;
                }

                void Serializer(BinaryWriter writer, object value)
                {
                    var array = (Array)value;
                    integerSerializer.Serialize(writer, array.Length);

                    for (int i = 0; i < array.Length; i++)
                    {
                        serializer.Serialize(writer, array.GetValue(i));
                    }
                }

                var arraySerializer = new Serializer(Deserializer, Serializer);
                serializers.Add(type, arraySerializer);
            }

            return base.Get(type);
        }

        public override bool Has(Type type)
        {
            return base.Has(type) || type.IsArray && Has(type.GetElementType());
        }

        public static readonly Serializer String =  new(r => r.ReadString(),    (w, v) => w.Write((string)v));
        public static readonly Serializer Char =    new(r => r.ReadChar(),      (w, v) => w.Write((char)v));
        public static readonly Serializer Boolean = new(r => r.ReadBoolean(),   (w, v) => w.Write((bool)v));

        public static readonly Serializer Byte =    new(r => r.ReadByte(),      (w, v) => w.Write((byte)v));
        public static readonly Serializer SByte =   new(r => r.ReadSByte(),     (w, v) => w.Write((sbyte)v));

        public static readonly Serializer Int16 =   new(r => r.ReadInt16(),     (w, v) => w.Write((short)v));
        public static readonly Serializer UInt16 =  new(r => r.ReadUInt16(),    (w, v) => w.Write((ushort)v));

        public static readonly Serializer Int32 =   new(r => r.ReadInt32(),     (w, v) => w.Write((int)v));
        public static readonly Serializer UInt32 =  new(r => r.ReadUInt32(),    (w, v) => w.Write((uint)v));

        public static readonly Serializer Int64 =   new(r => r.ReadInt64(),     (w, v) => w.Write((long)v));
        public static readonly Serializer UInt64 =  new(r => r.ReadUInt64(),    (w, v) => w.Write((ulong)v));

        public static readonly Serializer Single =  new(r => r.ReadSingle(),    (w, v) => w.Write((float)v));
        public static readonly Serializer Double =  new(r => r.ReadDouble(),    (w, v) => w.Write((double)v));
        public static readonly Serializer Decimal = new(r => r.ReadDecimal(),   (w, v) => w.Write((decimal)v));

        public Binary()
        {
            Add(new[]
            {
                (typeof(string),    String),
                (typeof(char),      Char),
                (typeof(bool),   Boolean),

                (typeof(byte),      Byte),
                (typeof(sbyte),     SByte),

                (typeof(short),     Int16),
                (typeof(ushort),    UInt16),

                (typeof(int),     Int32),
                (typeof(uint),    UInt32),

                (typeof(long),     Int64),
                (typeof(ulong),    UInt64),

                (typeof(float),    Single),
                (typeof(double),    Double),
                (typeof(decimal),   Decimal)
            });
        }
    }
}