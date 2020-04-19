using System;
using System.Collections.Generic;
using System.Reflection;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.Loaders
{
    public abstract class Loader<T> : IDisposable
    {
        protected Dictionary<Type, int> generics;
        protected T[] instances;


        protected Loader()
        {
            Initialize();
        }


        public void Initialize()
        {
            if (!PreInitialize())
                return;

            generics = new Dictionary<Type, int>();
            List<T> foundTypes = new List<T>();
            
            foreach (TypeInfo type in typeof(T).Assembly.Concrete<T>())
                if (LoadCondition(type))
                {
                    T instance = (T) Activator.CreateInstance(type);

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


        public virtual Predicate<TypeInfo> LoadCondition { get; } = obj => true;

        public Dictionary<Type, int>.KeyCollection LoadedTypes { get; private set; }

        public int Count => instances.Length;
        public T this[int index] => instances[index];
    }
}