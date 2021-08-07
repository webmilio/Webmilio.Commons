using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Webmilio.Commons.Extensions.Reflection;
using Webmilio.Commons.Helpers;

namespace Webmilio.Commons.Loaders
{
    public class PrototypeLoader<T> : IDisposable
    {
        protected Dictionary<Type, int> generics;
        protected T[] instances;


        public PrototypeLoader()
        {
            Initialize();
        }


        public void Initialize()
        {
            if (!PreInitialize())
                return;


            generics = new Dictionary<Type, int>();
            var foundTypes = new List<T>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (TypeInfo type in assembly.Concrete<T>())
                    if (LoadCondition(type))
                    {
                        T instance = TypeHelpers.Instantiate<T>();

                        if (!PreAdd(type, ref instance))
                            continue;

                        generics.Add(type, foundTypes.Count);
                        foundTypes.Add(instance);

                        PostAdd(type, instance);
                    }

            
            LoadedTypes = generics.Keys;
            instances = foundTypes.ToArray();


            PostInitialize();
        }

        public void Dispose()
        {
            generics = null;
            instances = null;
        }


        protected virtual bool PreInitialize() => true;
        protected virtual void PostInitialize() { }

        protected virtual bool PreAdd(TypeInfo type, ref T item) => true;
        protected virtual void PostAdd(TypeInfo type, T item) { }


        public TSub Generic<TSub>() where TSub : T => (TSub) Generic(typeof(T));
        public T Generic(Type type) => this [generics[type]];

        public int Id<TSub>() where TSub : T => generics[typeof(TSub)];
        public int Id(Type type) => generics[type];


        public IEnumerable<T> AsEnumerable() => instances.AsEnumerable();


        public virtual Predicate<TypeInfo> LoadCondition { get; } = obj => true;

        public Dictionary<Type, int>.KeyCollection LoadedTypes { get; private set; }


        public int Count => instances.Length;
        public T this[int index] => instances[index];
    }
}