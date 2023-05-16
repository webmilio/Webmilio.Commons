using System;
using System.Reflection;

namespace Webmilio.Commons.DependencyInjection;

public partial class ServiceCollection
{
    public void Map(Type root, Type current)
    {
        if (current == typeof(object))
        {
            return;
        }

        _redirects.Add(root, current);
        Map(root, current.BaseType);

        foreach (var interfaceType in current.GetInterfaces())
        {
            Map(root, interfaceType);
        }
    }

    public bool HasService(Type serviceType)
    {
        return 
            _iDescriptors.ContainsKey(serviceType) && 
            _redirects.ContainsKey(serviceType);
    }

    public bool HasMapping(Type serviceType)
    {
        return _redirects.ContainsKey(serviceType);
    }
}