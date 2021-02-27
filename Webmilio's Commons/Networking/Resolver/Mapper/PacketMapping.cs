using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Webmilio.Commons.Extensions;

namespace Webmilio.Commons.Networking.Resolver.Mapper
{
    public class PacketMapping
    {
        public PacketMapping(Type type)
        {
            Type = type;

            var properties = new List<PropertyMapping>();
            type.GetProperties().Do(delegate(PropertyInfo property)
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != default)
                    return;

                properties.Add(new PropertyMapping(property));
            });

            Mappings = properties.AsReadOnly();
        }


        public void Write(object instance, BinaryWriter writer)
        {
            Mappings.Do(m => m.Write(instance, writer));
        }

        public void Read(object instance, BinaryReader reader)
        {
            Mappings.Do(m => m.Read(instance, reader));
        }


        public Type Type { get; }

        public IReadOnlyList<PropertyMapping> Mappings { get; }


        public class PropertyMapping
        {
            public PropertyMapping(PropertyInfo property)
            {
                Property = property;

                Writer = BinaryIOHandlers.GetWritingMethod(property);
                Reader = BinaryIOHandlers.GetReadingMethod(property);
            }


            public void Write(object instance, BinaryWriter writer)
            {
                Writer(writer, Property.GetValue(instance));
            }

            public void Read(object instance, BinaryReader reader)
            {
                Property.SetValue(instance, Reader(reader));
            }


            public PropertyInfo Property { get; }

            public BinaryIOHandlers.WritingDelegate Writer { get; }
            public BinaryIOHandlers.ReadingDelegate Reader { get; }
        }
    }
}