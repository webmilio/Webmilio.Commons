using System;
using System.Collections.Generic;
using System.Reflection;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.DependencyInjection
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private readonly Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();

        private readonly Dictionary<Type, Func<ServiceProvider, object>> _factories = new Dictionary<Type, Func<ServiceProvider, object>>();
        private readonly Dictionary<Type, bool> _isSingleton = new Dictionary<Type, bool>();

        private readonly List<IServiceProvider> _providers = new List<IServiceProvider>();


        public ServiceProvider() : this(true)
        {
        }

        public ServiceProvider(params ServiceProvider[] parents) : this(false, parents)
        {
        }

        public ServiceProvider(bool mapServiceAttribute, params ServiceProvider[] parents)
        {
            AddInstance(this);
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


        public ServiceProvider AddSingleton<T>() => AddSingleton(typeof(T), null);
        public ServiceProvider AddSingleton<T>(Func<ServiceProvider, T> factory) => AddSingleton(typeof(T), provider => factory(provider));

        public ServiceProvider AddSingleton(Type serviceType) => AddSingleton(serviceType, null);
        public ServiceProvider AddSingleton(Type serviceType, Func<ServiceProvider, object> factory)
        {
            _factories.Add(serviceType, factory);
            _isSingleton.Add(serviceType, true);

            MapType(serviceType);

            return this;
        }

        public ServiceProvider AddInstance<T>(T instance)
        {
            _instances.Add(typeof(T), instance);
            MapType(typeof(T));

            return this;
        }

        public ServiceProvider AddTransient<T>() => AddTransient(typeof(T), null);
        public ServiceProvider AddTransient<T>(Func<ServiceProvider, T> factory) => AddTransient(typeof(T), provider => factory(provider));

        public ServiceProvider AddTransient(Type serviceType) => AddTransient(serviceType, null);
        public ServiceProvider AddTransient(Type serviceType, Func<ServiceProvider, object> factory)
        {
            _factories.Add(serviceType, factory);
            _isSingleton.Add(serviceType, false);

            MapType(serviceType);
            
            return this;
        }
        

        public ServiceProvider AddProvider(IServiceProvider provider)
        {
            if (provider == this)
                throw new InvalidOperationException("A service provider cannot contain itself as a provider, only as a service.");

            _providers.Add(provider);
            return this;
        }


        public bool RemoveInstance<T>()
        {
            return _map.TryGetValue(typeof(T), out var mapping) && _instances.Remove(mapping);
        }

        public bool RemoveInstance<T>(object instance)
        {
            if (!_map.TryGetValue(typeof(T), out var mapping))
                return false;

            if (!_instances.TryGetValue(mapping, out var iInstance))
                return false;

            return iInstance == instance && _instances.Remove(typeof(T));
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
            if (_constructors.TryGetValue(serviceType, out var c))
                return MakeFromConstructor(c);

            var match = FindConstructor(serviceType);

            if (match == default)
                throw new AmbiguousMatchException($"There are no constructors found which match the current available services for type {serviceType}.");

            _constructors.Add(serviceType, match);
            return MakeFromConstructor(match, match.GetParameters());
        }

        private ConstructorInfo FindConstructor(Type serviceType)
        {
            ParameterInfo[] matchParameters = default;
            ConstructorInfo match = default;

            foreach (var constructor in serviceType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                bool parameterMapped = true;

                for (int i = 0; i < parameters.Length && parameterMapped; i++)
                {
                    parameterMapped = HasMapping(parameters[i].ParameterType);
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
                if (_providers[i] is ServiceProvider sp && sp.HasMapping(serviceType)
                    || _providers[i].GetService(serviceType) != default)
                    return true;
            }

            return false;
        }


        public bool ServiceAttributeMapped { get; }
    }
}