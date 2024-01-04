using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Webmilio.Commons.DependencyInjection.Tests;

[TestClass]
public class ServiceCollection : BaseTest
{
    private interface IA { }

    private interface IB : IA { }

    private class H : IB { }

    [TestMethod]
    public void InterfaceMapping()
    {
        col = new()
        {
            Mapping =
            {
                MappingBehavior = MappingBehavior.Interfaces
            }
        };
        AddSingleton(typeof(H));

        Assert.IsTrue(col.Contains(typeof(IA)));
    }

    [TestMethod]
    public void InterfaceMapping_None()
    {
        col = new()
        {
            Mapping =
            {
                MappingBehavior = MappingBehavior.None
            }
        };
        AddSingleton(typeof(H));

        Assert.IsFalse(col.Contains(typeof(IA)));
    }
}
