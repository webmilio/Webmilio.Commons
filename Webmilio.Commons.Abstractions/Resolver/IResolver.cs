using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.Resolver;

public interface IResolver<T>
{
    IList<T> Resolve(IList<Assembly> assemblies);
}