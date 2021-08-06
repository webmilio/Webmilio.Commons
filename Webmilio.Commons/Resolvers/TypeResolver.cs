using System.Collections.Generic;
using System.Reflection;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.Resolvers
{
    public abstract class TypeResolver<T> : IResolver<TypeInfo>
    {
        public virtual IList<TypeInfo> Resolve(IList<Assembly> assemblies)
        {
            var types = new List<TypeInfo>();

            assemblies.Do(delegate (Assembly assembly)
            {
                var t = assembly.DefinedTypes.Concrete<T>();
                types.AddRange(t);

                t.DoEnumerable(Resolve);
            });

            PostResolve(assemblies, types);
            return types;
        }


        protected virtual void Resolve(TypeInfo type) { }

        protected virtual void PostResolve(IList<Assembly> assemblies, List<TypeInfo> types) { }
    }
}