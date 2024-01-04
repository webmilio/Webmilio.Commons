using System;

namespace Webmilio.Commons.DependencyInjection;

public interface IServiceFactory
{
    public object Make(Type serviceType, object[] binder);
}
