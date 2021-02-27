using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Webmilio.Commons.Networking.Resolver.Mapper
{
    public static class BinaryIOHandlers
    {
        public delegate void WritingDelegate(BinaryWriter writer, object value);
        public delegate object ReadingDelegate(BinaryReader reader);

        // TODO Account for subtypes.
        private static readonly Dictionary<Type, Handler> _handlers = new Dictionary<Type, Handler>();


        static BinaryIOHandlers()
        {
            // Numbers
            _handlers.Add(typeof(byte), new Handler(WriteByte, ReadByte));
            _handlers.Add(typeof(sbyte), new Handler(WriteSByte, ReadSByte));

            _handlers.Add(typeof(short), new Handler(WriteInt16, ReadInt16));
            _handlers.Add(typeof(ushort), new Handler(WriteUInt16, ReadUInt16));

            _handlers.Add(typeof(int), new Handler(WriteInt32, ReadInt32));
            _handlers.Add(typeof(uint), new Handler(WriteUInt32, ReadUInt32));

            _handlers.Add(typeof(long), new Handler(WriteInt64, ReadInt64));
            _handlers.Add(typeof(ulong), new Handler(WriteUInt64, ReadUInt64));

            _handlers.Add(typeof(float), new Handler(WriteFloat, ReadFloat));
            _handlers.Add(typeof(double), new Handler(WriteDouble, ReadDouble));
            _handlers.Add(typeof(decimal), new Handler(WriteDecimal, ReadDecimal));

            // Char(s)
            _handlers.Add(typeof(char), new Handler(WriteChar, ReadChar));
            _handlers.Add(typeof(string), new Handler(WriteString, ReadString));
        }


        public static WritingDelegate GetWritingMethod(PropertyInfo property) => GetWritingMethod(property.PropertyType);

        public static WritingDelegate GetWritingMethod(Type type)
        {
            if (type.IsArray)
            {
                var writingMethod = GetWritingMethod(type.GetElementType());

                return delegate(BinaryWriter writer, object value)
                {
                    var array = (Array) value;

                    writer.Write(array.Length);

                    for (int i = 0; i < array.Length; i++)
                        writingMethod(writer, array.GetValue(i));
                };
            }

            if (type.IsEnum)
                return GetWritingMethod(type.GetEnumUnderlyingType());

            if (_handlers.TryGetValue(type, out var handler))
                return handler.writingMethod;

            throw new NotImplementedException("The current type doesn't have a registered handler.");
        }

        public static ReadingDelegate GetReadingMethod(PropertyInfo property) => GetReadingMethod(property.PropertyType);

        public static ReadingDelegate GetReadingMethod(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var readingMethod = GetReadingMethod(elementType);

                return delegate(BinaryReader reader)
                {
                    var array = Array.CreateInstance(type.GetElementType(), reader.ReadInt32());

                    for (int i = 0; i < array.Length; i++)
                        array.SetValue(readingMethod(reader), i);

                    return array;
                };
            }

            if (type.IsEnum)
            {
                var readingMethod = GetReadingMethod(type.GetEnumUnderlyingType());
                return reader => Enum.ToObject(type, readingMethod(reader));
            }

            if (_handlers.TryGetValue(type, out var handler))
                return handler.readingMethod;

            throw new NotImplementedException("The current array type doesn't have a registered handler.");
        }


        // Numbers
        public static void WriteByte(BinaryWriter writer, object val) => writer.Write((byte)val);
        public static object ReadByte(BinaryReader reader) => reader.ReadByte();

        public static void WriteSByte(BinaryWriter writer, object val) => writer.Write((sbyte)val);
        public static object ReadSByte(BinaryReader reader) => reader.ReadSByte();

        public static void WriteInt16(BinaryWriter writer, object val) => writer.Write((short)val);
        public static object ReadInt16(BinaryReader reader) => reader.ReadInt16();

        public static void WriteUInt16(BinaryWriter writer, object val) => writer.Write((ushort)val);
        public static object ReadUInt16(BinaryReader reader) => reader.ReadUInt16();

        public static void WriteInt32(BinaryWriter writer, object val) => writer.Write((int)val);
        public static object ReadInt32(BinaryReader reader) => reader.ReadInt32();

        public static void WriteUInt32(BinaryWriter writer, object val) => writer.Write((uint)val);
        public static object ReadUInt32(BinaryReader reader) => reader.ReadUInt32();

        public static void WriteInt64(BinaryWriter writer, object val) => writer.Write((long)val);
        public static object ReadInt64(BinaryReader reader) => reader.ReadInt32();

        public static void WriteUInt64(BinaryWriter writer, object val) => writer.Write((ulong)val);
        public static object ReadUInt64(BinaryReader reader) => reader.ReadUInt64();

        public static void WriteFloat(BinaryWriter writer, object val) => writer.Write((float)val);
        public static object ReadFloat(BinaryReader reader) => reader.ReadSingle();

        public static void WriteDouble(BinaryWriter writer, object val) => writer.Write((double)val);
        public static object ReadDouble(BinaryReader reader) => reader.ReadSingle();

        public static void WriteDecimal(BinaryWriter writer, object val) => writer.Write((decimal)val);
        public static object ReadDecimal(BinaryReader reader) => reader.ReadSingle();


        // Char(s)
        public static void WriteChar(BinaryWriter writer, object val) => writer.Write((char)val);
        public static object ReadChar(BinaryReader reader) => reader.ReadChar();

        public static void WriteString(BinaryWriter writer, object val) => writer.Write((string)val);
        public static object ReadString(BinaryReader reader) => reader.ReadString();


        public static void RegisterType(Type type, WritingDelegate writingMethod, ReadingDelegate readingMethod)
        {
            if (_handlers.ContainsKey(type))
                throw new ArgumentException($"There is already a registration for type {type.FullName}.");

            _handlers.Add(type, new Handler(writingMethod, readingMethod));
        }


        public readonly struct Handler
        {
            public readonly WritingDelegate writingMethod;
            public readonly ReadingDelegate readingMethod;


            public Handler(WritingDelegate writingMethod, ReadingDelegate readingMethod)
            {
                this.writingMethod = writingMethod;
                this.readingMethod = readingMethod;
            }
        }
    }
}