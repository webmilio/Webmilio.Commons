using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.DependencyInjection
{
    public class ServiceCollection : IServiceContainer
    {
        private readonly Dictionary<Type, Type> _map = new();
        private readonly Dictionary<Type, object> _instances = new();

        private readonly Dictionary<Type, ConstructorInfo> _constructors = new();
        private readonly Dictionary<Type, List<InjectedProperty>> _properties = new();

        private readonly Dictionary<Type, Func<ServiceCollection, object>> _factories = new();
        private readonly Dictionary<Type, bool> _isSingleton = new();

        private readonly List<IServiceProvider> _providers = new();
        private readonly List<IServiceContainer> _container = new();


        public ServiceCollection() : this(true)
        {
        }

        public ServiceCollection(params ServiceCollection[] parents) : this(false, parents)
        {
        }

        public ServiceCollection(bool mapServiceAttribute, params ServiceCollection[] parents)
        {
            AddService(this);
            ServiceAttributeMapped = mapServiceAttribute;

            for (int i = 0; i < parents.Length; i++)
                AddProvider(parents[i]);

            if (!mapServiceAttribute)
                return;

            AppDomain.CurrentDomain.GetAssemblies().Concrete().DoEnumerable(t =>
            {
                var attribute = t.GetCustomAttribute<ServiceAttribute>();

                if (attribute == default)
                    return;

                switch (attribute.Type)
                {
                    case ServiceType.Singleton:
                        AddSingleton(t);
                        break;

                    case ServiceType.Transient:
                        AddTransient(t);
                        break;
                }
            });
        }


        #region IServiceContainer

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            AddSingleton(serviceType, c => callback(c, serviceType));
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            AddService(serviceType, callback);

            if (promote)
                _container.Do(c => c.AddService(serviceType, callback, true));
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            _AddService(serviceType, serviceInstance);
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            AddService(serviceType, serviceInstance);

            _container.Do(c =>
            {
                c.AddService(serviceType, serviceInstance, promote);
            });
        }

        public void RemoveService(Type serviceType, bool promote)
        {
            RemoveService(serviceType);

            if (promote)
                _container.Do(c => c.RemoveService(serviceType, promote));
        }

        #endregion

        public ServiceCollection AddSingleton<T>() => AddSingleton(typeof(T), null);
        public ServiceCollection AddSingleton<T>(Func<ServiceCollection, T> factory) => AddSingleton(typeof(T), provider => factory(provider));

        public ServiceCollection AddSingleton(Type serviceType) => AddSingleton(serviceType, null);
        public ServiceCollection AddSingleton(Type serviceType, Func<ServiceCollection, object> factory)
        {
            _factories.Add(serviceType, factory);
            _isSingleton.Add(serviceType, true);

            MapType(serviceType);

            return this;
        }

        public ServiceCollection AddService<T>(T instance) => _AddService(typeof(T), instance);

        private ServiceCollection _AddService(Type serviceType, object instance)
        {
            _instances.Add(serviceType, instance);
            MapType(serviceType);

            return this;
        }


        public ServiceCollection AddTransient<T>() => AddTransient(typeof(T), null);
        public ServiceCollection AddTransient<T>(Func<ServiceCollection, T> factory) => AddTransient(typeof(T), provider => factory(provider));

        public ServiceCollection AddTransient(Type serviceType) => AddTransient(serviceType, null);
        public ServiceCollection AddTransient(Type serviceType, Func<ServiceCollection, object> factory)
        {
            _factories.Add(serviceType, factory);
            _isSingleton.Add(serviceType, false);

            MapType(serviceType);

            return this;
        }


        public ServiceCollection AddProvider(IServiceProvider provider)
        {
            if (provider == this)
                throw new InvalidOperationException("A service provider cannot contain itself as a provider, only as a service.");

            _providers.Add(provider);
            return this;
        }

        public ServiceCollection AddContainer(IServiceContainer container)
        {
            if (container == this)
                throw new InvalidOperationException("A service container cannot contain itself as a container, only as a service.");

            _container.Add(container);
            return AddProvider(container);
        }


        public void RemoveService<T>()
        {
            RemoveService(typeof(T));
        }

        public void RemoveService(Type serviceType)
        {
            if (!_map.TryGetValue(serviceType, out var mapping))
                return;
            _instances.Remove(mapping);
            _map.Remove(mapping);
        }

        public T GetService<T>() => (T)GetService(typeof(T));

        public object GetService(Type serviceType)
        {
            object instance = default;

            if (_map.TryGetValue(serviceType, out var mapping))
                instance = GetOrMake(mapping);

            for (int i = 0; i < _providers.Count && instance == default; i++)
                instance = _providers[i].GetService(serviceType);

            return instance;
        }


        private object GetOrMake(Type serviceType)
        {
            if (_instances.TryGetValue(serviceType, out var service))
                return service;

            var instance = Make(serviceType);

            if (instance == default)
                throw new InvalidOperationException("Could not create an instance of the requested service.");

            if (_isSingleton[instance.GetType()])
                _instances.Add(serviceType, instance);

            return instance;
        }


        public T Make<T>() => (T)Make(typeof(T));

        public object Make(Type serviceType)
        {
            if (!HasMapping(serviceType)) // We support non-mapped types (aka not services).
            {
                return MapConstructorAndMake(serviceType);
            }

            if (_factories.ContainsKey(serviceType))
            {
                var factory = _factories[serviceType];

                return factory == null ? MapConstructorAndMake(serviceType) : factory(this);
            }

            return default;
        }

        private object MapConstructorAndMake(Type serviceType)
        {
            object instance;

            if (_constructors.TryGetValue(serviceType, out var c))
                instance = MakeFromConstructor(c);
            else
            {
                var match = FindConstructor(serviceType);

                if (match == default)
                    throw new AmbiguousMatchException($"There are no constructors found which match the current available services for type {serviceType}.");

                _constructors.Add(serviceType, match);
                instance = MakeFromConstructor(match, match.GetParameters());
            }


            if (!_properties.TryGetValue(serviceType, out var properties))
            {
                properties = GetInjectedProperties(serviceType);
                _properties.Add(serviceType, properties);
            }

            if (properties.Count > 0)
                InjectProperties(instance, properties);

            return instance;
        }

        private ConstructorInfo FindConstructor(Type serviceType)
        {
            return FindConstructor(serviceType, HasMapping);
        }

        internal static ConstructorInfo FindConstructor(Type serviceType, Func<Type, bool> mappingCheck)
        {
            ParameterInfo[] matchParameters = default;
            ConstructorInfo match = default;

            foreach (var constructor in serviceType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                bool parameterMapped = true;

                for (int i = 0; i < parameters.Length && parameterMapped; i++)
                {
                    parameterMapped = mappingCheck(parameters[i].ParameterType);
                }

                if (parameterMapped && (match == null || matchParameters.Length < parameters.Length))
                {
                    match = constructor;
                    matchParameters = parameters;
                }
            }

            return match;
        }

        private object MakeFromConstructor(ConstructorInfo constructor) => MakeFromConstructor(constructor, constructor.GetParameters());

        private object MakeFromConstructor(ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            return InjectServices(constructor, GetServices(parameters));
        }


        private void InjectProperties(object instance, List<InjectedProperty> properties)
        {
            properties.Do(ip =>
            {
                var service = GetService(ip.property.PropertyType);

                if (service == default && ip.attribute.Required)
                    throw new InvalidOperationException($"Service for property {ip.property.DeclaringType.Name}.{ip.property.Name} could not be acquired and property was marked as required.");

                ip.property.SetValue(instance, service);
            });
        }


        private object[] GetServices(IList<ParameterInfo> parameters)
        {
            var services = new object[parameters.Count];

            for (int i = 0; i < services.Length; i++)
                services[i] = GetService(parameters[i].ParameterType);

            return services;
        }

        private object InjectServices(ConstructorInfo constructor, object[] services)
        {
            return constructor.Invoke(services);
        }


        private void MapType(Type serviceType)
        {
            MapType(serviceType, serviceType);
            MapInterfaceType(serviceType, serviceType);
        }

        private void MapType(Type origin, Type current)
        {
            if (current == typeof(object))
                return;

            MapOrRemap(origin, current);
            MapType(origin, current.BaseType);
        }

        private void MapInterfaceType(Type origin, Type current)
        {
            if (current == typeof(IDisposable))
                return;

            foreach (var inter in current.GetInterfaces())
            {
                MapOrRemap(origin, inter);
                MapInterfaceType(origin, inter);
            }
        }

        private void MapOrRemap(Type origin, Type current)
        {
            if (_map.ContainsKey(current))
            {
                _map[current] = origin;

                if (_constructors.ContainsKey(current))
                    _constructors.Remove(current);
            }
            else
                _map.Add(current, origin);
        }

        private bool HasMapping(Type serviceType)
        {
            if (_map.ContainsKey(serviceType))
                return true;

            for (int i = 0; i < _providers.Count; i++)
            {
                if (_providers[i] is ServiceCollection sp && sp.HasMapping(serviceType)
                    || _providers[i].GetService(serviceType) != default)
                    return true;
            }

            return false;
        }

        private List<InjectedProperty> GetInjectedProperties(Type serviceType)
        {
            var properties = serviceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var injected = new List<InjectedProperty>(properties.Length);

            properties.Do(p =>
            {
                var attribute = p.GetCustomAttribute<ServiceAttribute>();

                if (attribute != default)
                    injected.Add(new InjectedProperty(p, attribute));
            });

            return injected;
        }


        public bool ServiceAttributeMapped { get; }


        private struct InjectedProperty
        {
            public readonly PropertyInfo property;
            public readonly ServiceAttribute attribute;


            public InjectedProperty(PropertyInfo property, ServiceAttribute attribute)
            {
                this.property = property;
                this.attribute = attribute;
            }
        }
    }

    public static class ServiceProviderExtensions
    {
        public static object GetService<T>(this IServiceProvider provider)
        {
            if (provider is ServiceCollection sc)
                return sc.GetService<T>();

            return provider.GetService(typeof(T));
        }

        public static T Make<T>(this IServiceProvider provider) => (T) Make(provider, typeof(T));

        public static object Make(this IServiceProvider provider, Type serviceType)
        {
            if (provider is ServiceCollection sc)
                return sc.Make(serviceType);

            var ctor = FindConstructable(provider, serviceType);

            if (ctor == default)
                return default;

            var parameters = ctor.GetParameters();
            var services = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                services[i] = provider.GetService(parameters[i].ParameterType);
            }

            return ctor.Invoke(services);
        }

        public static ConstructorInfo FindConstructable(this IServiceProvider provider, Type serviceType)
        {
            return ServiceCollection.FindConstructor(serviceType, t => provider.GetService(t) != default);
        }
    }
}