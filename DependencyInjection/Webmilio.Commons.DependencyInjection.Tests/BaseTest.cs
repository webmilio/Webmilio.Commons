using Microsoft.Extensions.DependencyInjection;

namespace Webmilio.Commons.DependencyInjection.Tests;

public class BaseTest
{
    protected DependencyInjection.ServiceCollection col;

    public class X { }
    public class Y(X x) : IA
    {
        public X x = x;
    }
    public class Z(X x, IA a) : IB 
    {
        public X x = x;
        public IA a = a;
    }

    public interface IA { }
    public interface IB { }

    [TestInitialize]
    public virtual void Initialize()
    {
        col = [];
    }

    public void AddSingleton(Type type)
    {
        col.AddService(new ServiceDescriptor(type, type, ServiceLifetime.Singleton));
    }
}
