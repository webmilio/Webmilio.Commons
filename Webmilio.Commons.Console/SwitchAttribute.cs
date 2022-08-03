using System;

namespace Webmilio.Commons.Console;

[AttributeUsage(AttributeTargets.Method)]
public class SwitchAttribute : Attribute
{
    public string[] Aliases { get; set; } = Array.Empty<string>();
}