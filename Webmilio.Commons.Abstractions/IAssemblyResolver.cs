using System.Collections.Generic;
using System.Reflection;

namespace Webmilio.Commons;

public interface IAssemblyResolver
{
    IList<Assembly> Resolve();
}