using NUnit.Framework;
using Webmilio.Commons.Helpers;

namespace Tests.Webmilio.Commons.Helpers
{
    [TestFixture]
    public class ObjectHelpersTests
    {
        [Test]
        public void PropertiesAllNull()
        {
            var sc = new TestClasses.SmallClass2()
            {
                Int1 = true,
                Int3 = 53,

                String1 = "lol",
                String3 = "sorry"
            };

            ObjectHelpers.ClearProperties(sc);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(default(bool), sc.Int1);
                Assert.AreEqual(default(int), sc.Int3);

                Assert.AreEqual(default(string), sc.String1);
                Assert.AreEqual(default(string), sc.String3);
            });
        }
    }
}