using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Webmilio.Commons.DependencyInjection.Tests.IServiceCollection;

[TestClass]
public class Transient : BaseTest
{
    [TestMethod]
    public void NotSameInstance()
    {
        col.AddService(new ServiceDescriptor(typeof(X), typeof(X), ServiceLifetime.Singleton));
        col.AddService(new ServiceDescriptor(typeof(Y), typeof(Y), ServiceLifetime.Transient));

        var x1 = col.GetService(typeof(Y));
        var x2 = col.GetService(typeof(Y));

        Assert.AreNotSame(x2, x1);
    }
}
