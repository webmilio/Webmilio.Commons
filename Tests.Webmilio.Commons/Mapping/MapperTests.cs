using NUnit.Framework;
using Webmilio.Commons.Mapping;

namespace Tests.Webmilio.Commons.Mapping
{
    [TestFixture]
    public class MapperTests
    {
        [Test]
        public void SameValues_AreEqual()
        {
            var sc = new TestClasses.SmallClass();
            var sc2 = Mapper.Common.Map<TestClasses.SmallClass2>(sc);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(sc2);

                Assert.AreEqual(sc.String1, sc2.String1);
                Assert.IsNull(sc2.String3);

                Assert.AreEqual(sc.Int2, sc2.Int2);
            });
        }
    }
}