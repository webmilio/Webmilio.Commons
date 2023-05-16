using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Webmilio.Commons.Extensions;

namespace Webmilio.Commons.DependencyInjection;

public partial class ServiceCollection : IServiceCollection
{
    private readonly Dictionary<Type, Type> _redirects = new();
    private readonly Dictionary<Type, ServiceDescriptor> _iDescriptors = new(); // Initial descriptors, provided by the user.
    private readonly Dictionary<Type, ServiceDescriptor> _cDescriptors = new(); // Completed descriptors, ready to be used in manufacturing services.

    public ServiceCollection AddService(ServiceDescriptor service)
    {
        _iDescriptors.Add(service.ServiceType, service);
        return this;
    }

    public ServiceCollection RemoveService(ServiceDescriptor service)
    {
        _iDescriptors.Remove(service.ServiceType);
        _cDescriptors.Remove(service.ServiceType);

        return this;
    }

    public object? Make(Type serviceType, params object[] dependencies)
    {

    }

    public object? Make(Type serviceType)
    {
        if (HasService(serviceType))
        {
            if (_cDescriptors.TryGetValueOrAdd(serviceType, out var descriptor, t => Complete(_iDescriptors[t])))
            {
                
            }
        }
        else // We're trying to make a service for an non-service type.
        {
            
        }
    }

    private object? Make(ConstructorInfo constructor)
    {

    }

    private ServiceDescriptor Complete(ServiceDescriptor service)
    {
        ServiceDescriptor descriptor;
        var constructor = FindConstructor(service.ServiceType);

        if (constructor == null)
        {
            throw new ConstructorMappingException($"No suitable constructor found to complete the specified service {service.ServiceType.FullName}.");
        }

        if (service.Lifetime == ServiceLifetime.Singleton)
        {
            var instance = Make(constructor);
            descriptor = new(service.ServiceType, (s) => instance, ServiceLifetime.Singleton);
        }
        else
        {
            descriptor = new(service.ServiceType, (s) => Make()
        }

        return descriptor;
    }

    private ConstructorInfo? FindConstructor(Type type)
    {
        ConstructorInfo? match = null;
        ParameterInfo[]? parameters = null;

        foreach (var constructor in type.GetConstructors())
        {
            var localParameters = constructor.GetParameters();
            bool pMapped = true;

            if (parameters != null && localParameters.Length < parameters.Length)
            {
                continue;
            }

            for (int i = 0; pMapped && i < localParameters.Length; i++)
            { 
                pMapped = HasService(localParameters[i].ParameterType) || localParameters[i].IsOptional;
                match = constructor;
            }
        }

        return match;
    }
}