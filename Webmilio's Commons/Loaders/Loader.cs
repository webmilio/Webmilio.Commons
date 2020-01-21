using System;
using System.Collections.Generic;
using System.Reflection;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.Loaders
{
    public abstract class Loader<T>
    {
        protected T[] instances;


        public void Initialize()
        {
            if (!PreInitialize())
                return;

            
            List<T> foundTypes = new List<T>();
            
            foreach (TypeInfo type in Assembly.GetExecutingAssembly().Concrete<T>())
                if (LoadCondition(type))
                    foundTypes.Add((T) Activator.CreateInstance(type));

            instances = foundTypes.ToArray();


            PostInitialize();
        }


        protected virtual bool PreInitialize() => true;

        protected virtual void PostInitialize() { }


        public virtual Predicate<TypeInfo> LoadCondition { get; } = obj => true;

        public int Count => instances.Length;

        public T this[int index] => instances[index];
    }
}