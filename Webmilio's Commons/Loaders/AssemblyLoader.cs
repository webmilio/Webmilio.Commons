using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Webmilio.Commons.Extensions.Reflection;
using Webmilio.Commons.Logging;

namespace Webmilio.Commons.Loaders
{
    public abstract class AssemblyLoader<T> where T : ILoadable
    {
        private Logger _logger;
        protected List<T> loaded;


        protected AssemblyLoader(DirectoryInfo contentDirectory)
        {
            ContentDirectory = contentDirectory;
        }


        public bool Load()
        {
            Logger.Log($"{GetType().Name} loading...");
            this.loaded = new List<T>();


            if (!PreLoad())
                return false;


            if (!ContentDirectory.Exists)
            {
                Logger.Info($"Content directory for {GetType().Name} not found; creating under {ContentDirectory.FullName}.");
                ContentDirectory.Create();
            }


            List<T> toLoad = new List<T>();

            foreach (FileInfo assemblyFile in ContentDirectory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                TypeInfo[] classes = Assembly.LoadFile(assemblyFile.FullName).Concrete<T>().ToArray();

                if (classes.Length == 0)
                {
                    Logger.Warning($"No class implementing {typeof(T).Name} were found in file {Path.GetFileName(assemblyFile.FullName)}.");
                    continue;
                }

                for (int i = 0; i < classes.Length; i++)
                {
                    T item = (T) Activator.CreateInstance(classes[i]);

                    toLoad.Add(item);
                    QueuedForLoad(item);
                }
            }

            foreach (T t in toLoad)
                if (t.Load())
                {
                    Logger.Info($"{typeof(T).Name} {t.GetType().Name} was successfully loaded.");
                    this.loaded.Add(t);

                    ItemLoaded(t);
                }
                else
                    Logger.Warning($"Error while loading {typeof(T).Name} {t.GetType().Name}.");

            bool preFinishLoading = PreFinishLoading();
            string typeName = GetType().Name;


            if (preFinishLoading)
                Logger.Log($"{typeName} finished loading: {this.loaded.Count} loaded.");
            else
                Logger.Warning($"{typeName} failed to load properly!");


            return preFinishLoading;
        }


        public virtual void QueuedForLoad(T item) { }
        public virtual void ItemLoaded(T item) { }

        public virtual bool PreLoad() => true;
        public virtual bool PreFinishLoading() => true;


        public IEnumerable<T> GetEnumerable() => loaded.ToArray();


        public Logger Logger
        {
            get => _logger ??= WebmiliosCommons.Logger;
            set => _logger = value;
        }

        public DirectoryInfo ContentDirectory { get; }

        public int Count => loaded.Count;


        public T this[int index] => loaded[index];
    }
}