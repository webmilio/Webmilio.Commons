using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.Resolvers;

public interface IResolver<T>
{
    IList<T> Resolve(IList<Assembly> assemblies);
}