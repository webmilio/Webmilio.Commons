using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.DependencyInjection;

public class ServiceCollection : IServiceCollection
{
    public class CacheProperties
    {
        public bool CacheConstructor { get; set; } = true;

        public bool InvalidateOnServiceRegistrationChange { get; set; }
        public bool CalculateOnRegistration { get; set; }
    }

    public class MappingProperties
    {
        public MappingBehavior MappingBehavior { get; set; } = MappingBehavior.All;
    }

    protected readonly Dictionary<Type, Type> typeMappings = [];
    protected readonly Dictionary<Type, ServiceDescriptor> descriptors = [];

    protected readonly Dictionary<Type, ServiceDescriptor> services = [];

    public CacheProperties Caching { get; } = new();
    public MappingProperties Mapping { get; } = new();

    public int Count => descriptors.Count;

    public bool IsReadOnly => false;

    #region Adding
    public void AddService(Type serviceType, ServiceCreatorCallback callback)
    {
        AddService(serviceType, callback, false);
    }

    public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
        AddService(new ServiceDescriptor(serviceType, _ => callback(this, serviceType), ServiceLifetime.Singleton));
    }

    public void AddService(Type serviceType, object serviceInstance)
    {
        AddService(serviceType, serviceInstance, false);
    }

    public void AddService(Type serviceType, object serviceInstance, bool promote)
    {
        AddService(new ServiceDescriptor(serviceType, serviceInstance));
    }

    // Only there so that there is every signature of 'AddService'.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddService(ServiceDescriptor descriptor) => Add(descriptor);

    public void Add(ServiceDescriptor item)
    {
        Add(item.ServiceType, item);
    }

    private void Add(Type rootType, ServiceDescriptor item)
    {
        item = MapDescriptor(item);
        descriptors.TryAdd(rootType, item);

        if (Caching.InvalidateOnServiceRegistrationChange)
        {
            UpdateRegistrations();
        }
    }

    public void Insert(int index, ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Removing
    public void RemoveService(Type serviceType)
    {
        var toRemove = new List<Type>();
        
        foreach (var kvp in typeMappings)
        {
            if (kvp.Value == serviceType)
            {
                toRemove.Add(kvp.Key);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            typeMappings.Remove(toRemove[i]);
        }

        descriptors.Remove(serviceType);
        services.Remove(serviceType);
    }

    public void RemoveService(Type serviceType, bool promote)
    {
        RemoveService(serviceType);
    }

    public void RemoveService(ServiceDescriptor descriptor) => Remove(descriptor);
    public bool Remove(ServiceDescriptor item)
    {
        if (!descriptors.TryGetValue(item.ServiceType, out var descriptor))
        {
            return false;
        }

        RemoveService(descriptor.ServiceType);
        return true;
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Getting
    public object GetService(Type serviceType)
    {
        if (!typeMappings.TryGetValue(serviceType, out var realServiceType))
        {
            throw new ServiceNotFoundException($"No mapping found for service type {serviceType}.");
        }

        if (services.TryGetValue(realServiceType, out var descriptor) && 
            descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        // If we get to here, we must build the service!
        if (!descriptors.TryGetValue(realServiceType, out descriptor))
        {
            throw new ServiceNotFoundException($"No service descriptor registered for service type {realServiceType}/{serviceType}.");
        }

        var instance = Make(serviceType);
        
        if (descriptor.Lifetime == ServiceLifetime.Singleton)
        {
            services.Add(realServiceType, new ServiceDescriptor(realServiceType, instance));
        }

        return instance;
    }

    public object GetRequiredService(Type serviceType)
    {
        var service = GetService(serviceType);

        if (service == null)
        {
            throw new ServiceNotFoundException($"Could not find the required service of type {serviceType}.");
        }

        return service;
    }
    #endregion

    #region Other
    public void UpdateRegistrations()
    {

    }

    public void Clear()
    {
        typeMappings.Clear();
        descriptors.Clear();
        services.Clear();
    }

    public bool Contains(ServiceDescriptor item)
    {
        return typeMappings.ContainsKey(item.ServiceType);
    }

    public bool Contains(Type serviceType)
    {
        return typeMappings.ContainsKey(serviceType);
    }

    public void CopyTo(ServiceDescriptor[] dst, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public int IndexOf(ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Mapping
    private static ConstructorInfo? FindConstructor(Type serviceType, List<Type> serviceTypes)
    {
        ConstructorInfo? ctr = null;
        ParameterInfo[]? parameters = null;

        foreach (var mCtr in serviceType.GetConstructors())
        {
            var mParameters = mCtr.GetParameters();

            int i;
            for (i = 0; i < mParameters.Length && serviceTypes.Contains(mParameters[i].ParameterType); i++) ;

            if (parameters == null ||
                i + 1 == mParameters.Length && mParameters.Length > parameters.Length)
            {
                ctr = mCtr;
                parameters = mParameters;
            }
        }

        return ctr;
    }

    private List<Type> GetServiceTypes()
    {
        return new List<Type>(typeMappings.Keys);
    }

    private ServiceDescriptor MapDescriptor(ServiceDescriptor descriptor)
    {
        var type = descriptor.ServiceType;

        if (Caching.CacheConstructor)
        {
            var services = GetServiceTypes();
            var ctor = FindConstructor(type, services);

            if (ctor == null)
            {
                throw new ConstructorNotFoundException($"Couldn't map type {type}: no constructor found compatible with registered services.");
            }



            descriptor = new ServiceDescriptor(descriptor.ServiceType, , descriptor.Lifetime);
        }

        MapType(descriptor.ServiceType, descriptor.ServiceType);
    }

    private void MapType(Type root, Type curr)
    {
        var behavior = Mapping.MappingBehavior;

        if (behavior == MappingBehavior.None ||
            curr == typeof(object) || curr == typeof(IDisposable))
        {
            return;
        }

        typeMappings.TryAdd(curr, root);

        if (behavior.HasFlag(MappingBehavior.Interfaces))
        {
            foreach (var intr in curr.GetInterfaces())
            {
                MapType(root, intr);
            }
        }

        if (behavior.HasFlag(MappingBehavior.Classes))
        {
            if (curr.BaseType != null)
            {
                MapType(root, curr.BaseType);
            }
        }
    }

    private ConstructorInfo? FindConstructor(Type serviceType) => FindConstructor(serviceType, GetServiceTypes());
    #endregion

    #region Making
    public object Make(Type serviceType, object[] binder)
    {
        if (!typeMappings.TryGetValue(serviceType, out serviceType))
        {
            throw new ServiceNotFoundException($"Tried instanciating service type {serviceType} that is not registered.");
        }

        return MakeDirect(serviceType, binder);
    }

    private List<Type> GetBinder(object[] binder)
    {
        var serviceTypes = GetServiceTypes();
        for (int i = 0; i < binder.Length; i++)
        {
            serviceTypes.Add(binder[i].GetType());
        }

        return serviceTypes;
    }

    public object MakeDirect(Type serviceType, object[] binder)
    {
        var serviceTypes = GetBinder(binder);
        var constructor = FindConstructor(serviceType, serviceTypes);

        if (constructor == null)
        {
            throw new ConstructorNotFoundException();
        }

        binder = MakeBinder(constructor.GetParameters().GetTypes(), binder);
        return Make(constructor, binder);
    }

    private object MakeDirect(ConstructorInfo constructor, object[] binder)
    {
        var serviceTypes = GetServiceTypes();
        for (int i = 0; i < binder.Length; i++)
        {
            serviceTypes.Add(binder[i].GetType());
        }

        binder = MakeBinder(constructor.GetParameters().GetTypes());
        return Make(constructor, binder);
    }

    private object Make(ConstructorInfo creator, object[] binder)
    {
        return creator.Invoke(binder);
    }

    public object[] MakeBinder([NotNull] Type[] serviceTypes) => MakeBinder(serviceTypes, []);
    public object[] MakeBinder([NotNull] Type[] serviceTypes, object[] extra)
    {
        var instances = new object[serviceTypes.Length];
        var map = new Dictionary<Type, ServiceDescriptor>(descriptors);

        for (int i = 0; i < extra.Length; i++)
        {
            var type = extra[i].GetType();
            map.Add(type, new(type, extra[i]));
        }

        for (int i = 0; i < serviceTypes.Length; i++)
        {
            instances[i] = GetService(serviceTypes[i]);
        }

        return instances;
    }

    public object Make(Type serviceType) => Make(serviceType, []);
    #endregion

    public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
