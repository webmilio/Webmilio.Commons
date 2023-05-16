using Microsoft.Extensions.DependencyInjection;
using System;

namespace Webmilio.Commons.DependencyInjection;

public partial class ServiceCollection : IServiceProvider
{
    public object? GetService(Type serviceType)
    {
        ServiceDescriptor descriptor;

        if (!_cDescriptors.ContainsKey(serviceType) && _iDescriptors.TryGetValue(serviceType, out descriptor))
        {
            var cDescriptor = Complete(descriptor);

            if (cDescriptor != null)
            {
                _cDescriptors.Add(serviceType, cDescriptor);
            }
        }

        if (_cDescriptors.TryGetValue(serviceType, out descriptor))
        {
            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton: return descriptor.ImplementationInstance;
                default: return descriptor.ImplementationFactory!(this); // We treat scoped and instanced the same outside of a ScopeContext.
            }
        }

        // Build the complete descriptor.

        return null;
    }
}