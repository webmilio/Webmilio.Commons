using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Webmilio.Commons.Resolvers;

namespace Webmilio.Commons.Assemblies
{
    public class PluginAssembliesResolver : IAssemblyResolver
    {
        private IList<Assembly> _assemblies;


        public virtual IList<Assembly> Resolve()
        {
            var assemblies = new List<Assembly>();

            foreach (var file in Directory.GetFiles("Plugins", "*.dll", SearchOption.TopDirectoryOnly))
                assemblies.Add(Assembly.LoadFile(file));

            return assemblies.ToArray();
        }


        public IList<Assembly> Assemblies
        {
            get
            {
                return _assemblies ??= Resolve();
            }
        }
    }
}