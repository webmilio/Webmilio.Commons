Small abstract library required for Webmilio.Commons.DependencyInjection.Abstraction.
Contains various classes, such as exceptions used during dependency injection container initialization, enums for settings
and the ICompleteServiceCollection (for the lack of a better name) interface which implements:
* Microsoft.Extensions.DependencyInjection.IServiceCollection
* System.ComponentModel.Design.IServiceContainer
* Microsoft.Extensions.DependencyInjection.ISupportRequiredService
* Webmilio.Commons.DependencyInjection.IServiceFactory