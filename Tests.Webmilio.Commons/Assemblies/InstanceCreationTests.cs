using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Webmilio.Commons.Extensions.Reflection;

namespace Tests.Webmilio.Commons.Assemblies;

[TestFixture]
public class InstanceCreationTests
{
    [Test]
    public void CreateInstance()
    {
        Z instance = null;
        Assert.DoesNotThrow(() => instance = (Z) typeof(Z).Create());
        Assert.IsNotNull(instance);
    }

    [Test]
    public void CreateInstances()
    {
        IEnumerable<object> instances = null;
        Assert.DoesNotThrow(() => instances = Assembly.GetExecutingAssembly().Concrete<X>().Create());
        Assert.IsNotNull(instances);
        Assert.IsNotEmpty(instances);
    }

    [Test]
    public void CreateGeneric()
    {
        Z instance = null;
        Assert.DoesNotThrow(() => instance = typeof(Z).Create<Z>());
        Assert.IsNotNull(instance);
    }

    [Test]
    public void CreateGenerics()
    {
        IEnumerable<X> instances = null;
        Assert.DoesNotThrow(() => instances = Assembly.GetExecutingAssembly().Concrete<X>().Create<X>());
        Assert.IsNotNull(instances);
        Assert.IsNotEmpty(instances);
    }
}