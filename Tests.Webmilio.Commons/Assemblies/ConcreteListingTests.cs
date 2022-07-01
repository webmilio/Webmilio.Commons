using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Webmilio.Commons.Extensions.Reflection;

namespace Tests.Webmilio.Commons.Assemblies;

[TestFixture]
public class ConcreteListingTests
{
    [Test]
    public void Concretes()
    {
        IEnumerable<TypeInfo> types = null;
        Assert.DoesNotThrow(() => types = Assembly.GetExecutingAssembly().Concrete());
        Assert.IsNotEmpty(types);
    }

    [Test]
    public void Generics()
    {
        IEnumerable<TypeInfo> types = null;
        Assert.DoesNotThrow(() => types = Assembly.GetExecutingAssembly().Concrete<ITest>());
        Assert.IsNotEmpty(types);
        Assert.IsTrue(types.Count() >= 2);
    }

    [Test]
    public void AppDomainConcrete()
    {
        Assert.IsNotEmpty(AppDomain.CurrentDomain.GetAssemblies().Concrete<ITest>());
    }

    public interface ITest { }
    public class X : ITest { }
    public class Y : ITest { }
    public class Z : ITest { }
}