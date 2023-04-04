using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webmilio.Commons.DependencyInjection;

namespace DependencyInjection.Tests;

[TestClass]
public class ServiceCollectionTests
{
    private ServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new();
        _services
            .AddSingleton<Y>()
            .AddSingleton<Z>()
            
            .AddTransient<A>();
    }

    [TestMethod]
    public void GetSingleton()
    {
        var y = _services.GetService<Y>();
        
        Assert.IsNotNull(y);
        Assert.IsNotNull(y.z);
    }

    [TestMethod]
    public void GetRequiredService()
    {
        Assert.ThrowsException<InvalidOperationException>(_services.GetRequiredService<X>);
    }

    [TestMethod]
    public void GetTransient()
    {
        var i1 = _services.GetService<A>();
        var i2 = _services.GetService<A>();

        Assert.AreNotEqual(i1, i2);
    }

    [TestMethod]
    public void Make()
    {
        var x = _services.Make<X>();
        
        Assert.IsNotNull(x);
        Assert.IsNotNull(x.y);
        Assert.IsNotNull(x.y.z);
    }

    private class X
    {
        internal Y y;

        public X(Y y)
        {
            this.y = y;
        }
    }

    private class Y
    {
        internal Z z;

        public Y(Z z)
        {
            this.z = z;
        }
    }

    private class Z
    {

    }

    private class A
    {
        internal Z z;

        public A(Z z)
        {
            this.z = z;
        }
    }
}