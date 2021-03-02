using System.Reflection;

namespace Webmilio.Commons.Resolvers
{
    public interface IResolver
    {
        void Resolve(Assembly[] assemblies);
    }
}