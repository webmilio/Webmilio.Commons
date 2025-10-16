using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.Design;

namespace Webmilio.Commons.DependencyInjection;

public interface ICompleteServiceCollection : IServiceCollection, 
    IServiceContainer, IServiceFactory, ISupportRequiredService, IServiceProviderIsService
{
}