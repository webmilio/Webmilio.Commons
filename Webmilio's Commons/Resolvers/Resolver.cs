using System.Reflection;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.Resolvers
{
    public abstract class Resolver<T> : IResolver
    {
        public virtual void Resolve(params Assembly[] assemblies)
        {
            assemblies.Do(delegate (Assembly assembly)
            {
                assembly.DefinedTypes.Concrete<T>().Do(Resolve);
            });
        }

        protected abstract void Resolve(TypeInfo type);

        protected virtual void PostResolve(Assembly[] assemblies) { }
    }
}