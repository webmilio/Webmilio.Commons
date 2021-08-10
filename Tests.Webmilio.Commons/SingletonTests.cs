using NUnit.Framework;
using Webmilio.Commons;

namespace Tests.Webmilio.Commons
{
    [TestFixture]
    public class SingletonTests
    {
        [Test]
        public void Instance_ReturnSameInstance()
        {
            var instance1 = Singleton<SingletonClass>.Instance;
            var instance2 = Singleton<SingletonClass>.Instance;

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void HasInstance_ReturnFalse()
        {
            Assert.IsFalse(Singleton<SingletonClass_NotInitiated>.HasInstance);
        }

        [Test]
        public void HasInstance_ReturnTrue()
        {
            _ = Singleton<SingletonClass_Initiated>.Instance;
            Assert.IsTrue(Singleton<SingletonClass_Initiated>.HasInstance);
        }

        [Test]
        public void LoseInstance_HasInstance_ReturnFalse()
        {
            _ = Singleton<SingletonClass_Lose>.Instance;
            Assert.IsTrue(Singleton<SingletonClass_Lose>.HasInstance);

            Singleton<SingletonClass_Lose>.Lose();
            Assert.IsFalse(Singleton<SingletonClass_Lose>.HasInstance);
        }


        private class SingletonClass { }

        private class SingletonClass_Lose { }

        private class SingletonClass_NotInitiated { }

        private class SingletonClass_Initiated { }
    }
}