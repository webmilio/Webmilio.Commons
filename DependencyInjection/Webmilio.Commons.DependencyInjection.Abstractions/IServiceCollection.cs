using System;
using System.ComponentModel.Design;

namespace Webmilio.Commons.DependencyInjection;

public interface IServiceCollection : IServiceContainer
{
    public object Make(Type serviceType);
}