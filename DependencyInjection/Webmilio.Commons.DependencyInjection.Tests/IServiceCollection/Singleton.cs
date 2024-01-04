using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webmilio.Commons.DependencyInjection.Tests;

namespace Webmilio.Commons.DependencyInjection.Tests.IServiceCollection;

[TestClass]
public class Singleton : BaseTest
{
    [TestMethod]
    public void AddService_Descriptor()
    {
        col.AddService(new ServiceDescriptor(typeof(X), typeof(X), ServiceLifetime.Singleton));
        col.AddService(new ServiceDescriptor(typeof(Y), typeof(Y), ServiceLifetime.Singleton));

        var y = col.GetService(typeof(Y)) as Y;
        AssertInstances(y);
    }

    [TestMethod]
    public void AddService_ServiceInstance()
    {
        col.AddService(typeof(X), new X());

        Assert.IsNotNull(col.GetService(typeof(X)) as X);
    }

    [TestMethod]
    public void AddService_ServiceCreatorCallback()
    {
        col.AddService(typeof(X), (s, t) => new X());
        col.AddService(typeof(Y), (s, t) => new Y(s.GetService(typeof(X)) as X));

        Assert.IsNotNull(col.GetService(typeof(X)) as X);
    }

    [TestMethod]
    public void Interfaces()
    {
        AddSingleton(typeof(X));
        AddSingleton(typeof(Y));
        AddSingleton(typeof(Z));

        var z = col.GetService(typeof(Z)) as Z;

        Assert.IsNotNull(z);
        Assert.IsNotNull(z.a);
        Assert.IsNotNull(z.x);
    }

    private static void AssertInstances(Y? x)
    {
        Assert.IsNotNull(x);
        Assert.IsNotNull(x.x);
    }
}
