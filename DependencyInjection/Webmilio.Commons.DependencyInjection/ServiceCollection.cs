using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.DependencyInjection;

public class ServiceCollection : IServiceCollection
{
    public class CacheProperties
    {
        public bool CacheConstructorOnGetService { get; set; } = true;

        public bool InvalidateOnServiceRegistrationChange { get; set; }
        public bool CalculateOnRegistration { get; set; }
    }

    public class MappingProperties
    {
        public bool CacheConstructorOnMapping { get; set; }

        public MappingBehavior MappingBehavior { get; set; } = MappingBehavior.All;

        public BindingFlags ConstructorFlags { get; set; } = BindingFlags.Instance | BindingFlags.Public;
    }

    protected readonly Dictionary<Type, Type> mappings = [];
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
        MapType(rootType, rootType);

        // Implies that the item is a singleton.
        if (item.ImplementationInstance != null)
        {
            services.Add(rootType, item);
        }
        else
        {
            if (Mapping.CacheConstructorOnMapping)
            {
                item = CreateDescriptorFromConstructor(item, []);
            }

            if (Caching.InvalidateOnServiceRegistrationChange)
            {
                UpdateRegistrations();
            }
        }

        descriptors.TryAdd(rootType, item);
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

        foreach (var kvp in mappings)
        {
            if (kvp.Value == serviceType)
            {
                toRemove.Add(kvp.Key);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            mappings.Remove(toRemove[i]);
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
        if (!mappings.TryGetValue(serviceType, out var realServiceType))
        {
            throw new ServiceNotFoundException($"No mapping found for service type {serviceType}.");
        }

        if (services.TryGetValue(realServiceType, out var descriptor) &&
            descriptor.ImplementationInstance != null)
        {
            // An instance is already defined, return it.
            return descriptor.ImplementationInstance;
        }

        // If we get to here, we must build the service!
        if (!descriptors.TryGetValue(realServiceType, out descriptor))
        {
            throw new ServiceNotFoundException($"No service descriptor registered for service type {realServiceType}/{serviceType}.");
        }

        return GetService(realServiceType, descriptor);
    }

    private object GetService(Type type, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationFactory == null)
        {
            // We don't know how to 
            descriptor = CreateDescriptorFromConstructor(descriptor);
        }

        var instance = GetOrMake(descriptor);

        if (Caching.CacheConstructorOnGetService && descriptor.Lifetime == ServiceLifetime.Singleton)
        {
            services.Add(type, new ServiceDescriptor(type, instance));
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

    public object GetService(ServiceDescriptor descriptor) => GetService(descriptor.ServiceType, descriptor);
    #endregion

    #region Other
    public void UpdateRegistrations()
    {

    }

    public void Clear()
    {
        mappings.Clear();
        descriptors.Clear();
        services.Clear();
    }

    public bool Contains(ServiceDescriptor item)
    {
        return mappings.ContainsKey(item.ServiceType);
    }

    public bool Contains(Type serviceType)
    {
        return mappings.ContainsKey(serviceType);
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
    private List<Type> GetAvailableTypes()
    {
        return new List<Type>(mappings.Keys);
    }

    private ServiceDescriptor CreateDescriptorFromConstructor(ServiceDescriptor descriptor, IList<Type> extraTypes)
    {
        var type = descriptor.ServiceType;

        var availableServices = GetAvailableTypes();
        availableServices.AddRange(extraTypes);

        Func<IServiceProvider, object>? factory = descriptor.ImplementationFactory;

        if (factory == null)
        {
            if (!TryFindConstructor(type, availableServices, out var ctor, out var @params))
            {
                throw new ConstructorNotFoundException($"Couldn't map type {type}: no constructor found compatible with registered services.");
            }

            var requiredTypes = @params.GetTypes();

            var binder = PopulateBinder(requiredTypes);
            factory = delegate (IServiceProvider services)
            {
                return ctor.Invoke(binder);
            };
        }

        return new ServiceDescriptor(descriptor.ServiceType, factory, descriptor.Lifetime);
    }

    private void MapType(Type root, Type curr)
    {
        var behavior = Mapping.MappingBehavior;

        if (behavior == MappingBehavior.None ||
            curr == typeof(object) || curr == typeof(IDisposable))
        {
            return;
        }

        mappings.TryAdd(curr, root);

        MapInterfaces(root, curr);
        MapClasses(root, curr);
    }

    private void MapInterfaces(Type root, Type curr)
    {
        if (Mapping.MappingBehavior.HasFlag(MappingBehavior.Interfaces))
        {
            foreach (var intr in curr.GetInterfaces())
            {
                MapType(root, intr);
            }
        }
    }

    private void MapClasses(Type root, Type curr)
    {
        if (Mapping.MappingBehavior.HasFlag(MappingBehavior.Classes))
        {
            static bool Continue(Type? type) => type != null && type != typeof(object);

            do
            {
                mappings.TryAdd(curr, root);
            }
            while (Continue(curr = root.BaseType));
        }
    }

    public ServiceDescriptor CreateDescriptorFromConstructor(ServiceDescriptor descriptor) => CreateDescriptorFromConstructor(descriptor, []);
    #endregion

    #region Making
    /// <summary>Creates an object binder for the provided types.</summary>
    /// <param name="types">The types for the object binder.</param>
    /// <param name="binder">The extra object instances available to create the binder.</param>
    private object[] PopulateBinder(IList<Type> types, object[] binder)
    {
        var mDescriptors = new Dictionary<Type, ServiceDescriptor>(descriptors);
        for (int i = 0; i < binder.Length; i++)
        {
            mDescriptors.Add(binder[i].GetType(), new ServiceDescriptor(binder[i].GetType(), binder[i]));
        }

        binder = new object[types.Count];

        for (int i = 0; i < types.Count; i++)
        {

            binder[i] = GetService(types[i]);
        }

        return binder;
    }

    public object Make(Type serviceType, params object[] binder)
    {
        var descriptor = new ServiceDescriptor(serviceType, serviceType, ServiceLifetime.Transient);
        var availableTypes = new Type[binder.Length];

        for (int i = 0; i < binder.Length; i++)
        {
            availableTypes[i] = binder[i].GetType();
        }

        descriptor = CreateDescriptorFromConstructor(descriptor, availableTypes);
        return GetOrMake(descriptor);
    }

    private object GetOrMake(ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        if (descriptor.ImplementationFactory == null)
        {
            throw new NullReferenceException("No implementation factory given when trying to create an instance from a service descriptor.");
        }

        return Make(descriptor);
    }

    public object Make(ServiceDescriptor descriptor)
    {
        descriptor = CreateDescriptorFromConstructor(descriptor, []);

        if (descriptor.ImplementationFactory == null)
        {
            throw new ConstructorNotFoundException($"Could not define the implementation factory for service type {descriptor.ServiceType}.");
        }

        var instance = descriptor.ImplementationFactory(this);
        return instance;
    }

    private bool TryFindConstructor(Type type, IList<Type> availableTypes, out ConstructorInfo? ctor, out ParameterInfo[]? parms)
    {
        ctor = null;
        parms = null;

        int lenParms = -1;
        var ctors = type.GetConstructors(Mapping.ConstructorFlags);

        for (int i = 0; i < ctors.Length; i++)
        {
            int matchedParameters = 0;
            var mParameters = ctors[i].GetParameters();

            foreach (var param in mParameters)
            {
                if (availableTypes.Contains(param.ParameterType))
                {
                    matchedParameters++;
                }
            }

            if (lenParms < matchedParameters)
            {
                ctor = ctors[i];
                parms = mParameters;
            }
        }

        return ctor != null;
    }

    private object[] PopulateBinder(IList<Type> types) => PopulateBinder(types, []);
    public object Make(Type serviceType) => Make(serviceType, []);
    #endregion

    public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
