using System;

namespace Webmilio.Commons.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceType type = ServiceType.Singleton)
        {
            Type = type;
        }


        public ServiceType Type { get; }

        /// <summary>
        /// <c>true</c> if an error should be thrown when trying to inject a dependency into a property; <c>false</c> otherwise.
        /// Default: <c>true</c>
        /// </summary>
        public bool Required { get; set; } = true;
    }
}