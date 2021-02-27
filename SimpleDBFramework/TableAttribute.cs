using System;

namespace WebmilioCommons.SQLiteFramework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableAttribute : Attribute
    {
        public TableAttribute()
        {
        }

        public TableAttribute(string name)
        {
            Name = name;
        }


        public string Name { get; set; }
    }
}