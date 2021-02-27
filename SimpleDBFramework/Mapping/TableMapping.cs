using System.Reflection;

namespace WebmilioCommons.SQLiteFramework.Mapping
{
    public class TableMapping
    {
        public TableMapping(PropertyInfo property)
        {
            Property = property;
            var tableAttribute = Property.GetCustomAttribute<TableAttribute>();

            Name = tableAttribute?.Name;

            if (string.IsNullOrWhiteSpace(Name))
                Name = property.Name;
        }


        public PropertyInfo Property { get; }

        public string Name { get; }
    }
}