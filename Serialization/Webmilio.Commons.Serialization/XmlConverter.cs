using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.Serialization;

public class XmlConverter
{
    private readonly XmlSerializationOptions _options;
    private readonly PropertyInfo[] _properties;

    public XmlConverter(Type type) : this(type, new()) { }

    public XmlConverter(Type type, XmlSerializationOptions options)
    {
        Type = type;
        _options = options;

        _properties =
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
    }

    public object Deserialize(XmlNode node)
    {
        var instance = Activator.CreateInstance(Type);

        bool Find(XmlAttributeCollection attributes, string name, out XmlAttribute attribute)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    attribute = attributes[i];
                    return true;
                }
            }

            attribute = null;
            return false;
        }

        void SetValue(PropertyInfo property, string value)
        {
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(instance, value);
                return;
            }

            var converted = _options.Convert(property.PropertyType, value);

            if (converted == null && Nullable.GetUnderlyingType(property.PropertyType) != null)
                throw new SerializationException($"Failed to assign value to {property.Name}.");

            property.SetValue(instance, converted);
        }

        foreach (var property in _properties)
        {
            var name = property.Name;

            if (property.TryGetCustomAttribute(out NameAttribute n))
                name = n.Name;

            if (Find(node.Attributes, name, out var attribute))
                SetValue(property, attribute.Value);
            else if (name.Equals("value", StringComparison.OrdinalIgnoreCase))
                SetValue(property, node.Value);
        }

        return instance;
    }

    public Type Type { get; }
}