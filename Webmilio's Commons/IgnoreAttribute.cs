using System;

namespace Webmilio.Commons
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        
    }
}