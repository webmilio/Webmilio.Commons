using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons.Assemblies;

public interface IAssemblyResolver
{
    IList<Assembly> Resolve();
}