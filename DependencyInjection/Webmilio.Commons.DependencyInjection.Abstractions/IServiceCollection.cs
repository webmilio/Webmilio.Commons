using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.Design;

namespace Webmilio.Commons.DependencyInjection;

public interface IServiceCollection : Microsoft.Extensions.DependencyInjection.IServiceCollection, 
    IServiceContainer, IServiceFactory, ISupportRequiredService
{
}