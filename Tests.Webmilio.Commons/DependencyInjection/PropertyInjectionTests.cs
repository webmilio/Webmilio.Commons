using System;
using NUnit.Framework;
using Webmilio.Commons.DependencyInjection;

namespace Tests.Webmilio.Commons.DependencyInjection
{
    [TestFixture]
    public class PropertyInjectionTests
    {
        [Test]
        public void AutoInjectProperty_Required()
        {
            var services = new ServiceProvider();

            services.AddSingleton<Class1>();
            services.AddSingleton<Class2>();
            services.AddSingleton<Class3>();
            services.AddSingleton<Class5>();

            var c5 = services.GetService<Class5>();

            Assert.Multiple(() =>
            {
                Assert.NotNull(c5.C2);
                Assert.NotNull(c5.C3);
                Assert.Null(c5.C4);
            });
        }

        [Test]
        public void AutoInjectProperty_RequiredThrows()
        {
            var services = new ServiceProvider();

            services.AddSingleton<Class1>();
            services.AddSingleton<Class2>();
            services.AddSingleton<Class5>();

            Assert.Throws<InvalidOperationException>(() => services.GetService<Class5>());
        }

        private class Class1
        {
            public int X { get; } = 1;
        }

        private class Class2
        {
            private Class1 _c1;

            public Class2(Class1 c1)
            {
                _c1 = c1;
            }
        }

        private class Class3
        {
        }

        private class Class4
        {
        }

        private class Class5
        {
            [Service]
            public Class2 C2 { get; set; }

            [Service]
            public Class3 C3 { get; set; }

            [Service(Required = false)]
            public Class4 C4 { get; set; }
        }
    }
}