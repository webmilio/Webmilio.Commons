using System;
using System.ComponentModel.Design;

namespace Webmilio.Commons.DependencyInjection;

public partial class ServiceCollection : IServiceContainer
{
    public void AddService(Type serviceType, ServiceCreatorCallback callback)
    {
        throw new NotImplementedException();
    }

    public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
        throw new NotImplementedException();
    }

    public void AddService(Type serviceType, object serviceInstance)
    {
        throw new NotImplementedException();
    }

    public void AddService(Type serviceType, object serviceInstance, bool promote)
    {
        throw new NotImplementedException();
    }

    public void RemoveService(Type serviceType)
    {
        throw new NotImplementedException();
    }

    public void RemoveService(Type serviceType, bool promote)
    {
        throw new NotImplementedException();
    }
}