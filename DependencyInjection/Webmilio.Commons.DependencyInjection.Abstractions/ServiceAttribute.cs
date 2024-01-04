using Microsoft.Extensions.DependencyInjection;
using System;

namespace Webmilio.Commons.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class ServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// <c>true</c> if an error should be thrown when trying to inject a dependency into a property; <c>false</c> otherwise.
    /// Default: <c>true</c>
    /// </summary>
    public bool Required { get; set; } = true;

    public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        Lifetime = lifetime;
    }
}