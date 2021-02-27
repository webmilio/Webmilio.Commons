using System;

namespace Webmilio.Commons.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceType type = ServiceType.Singleton)
        {
            Type = type;
        }


        public ServiceType Type { get; }
    }
}