using System;

namespace Webmilio.Commons;

public class NameAttribute : Attribute
{
    public NameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}