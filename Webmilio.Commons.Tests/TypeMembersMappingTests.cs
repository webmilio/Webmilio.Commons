using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webmilio.Commons.Mappings;
using Webmilio.Commons.Serialization;

namespace Webmilio.Commons.Tests;

[TestClass]
public class TypeMembersMappingTests
{
    private TestTBR _tbr;

    [TestInitialize]
    public void Initialize()
    {
        _tbr = new TestTBR();
    }

    [TestMethod]
    public void PrivatePropertyCount()
    {
        var tmm = new TypeMembersMapping<TestTBR.ExpectedResult>(typeof(BooleanPropertyTest), _tbr);
        Assert.AreEqual(1, tmm.Wrappers.Count);
    }

    [TestMethod]
    public void MapTestString()
    {
        var tmm = new TypeMembersMapping<TestTBR.ExpectedResult>(typeof(StringPropertyTest), _tbr);
        Assert.AreEqual(TestTBR.ExpectedResult.String, tmm.Wrappers[0].mapping);
    }

    [TestMethod]
    public void MapTestInt()
    {
        var tmm = new TypeMembersMapping<TestTBR.ExpectedResult>(typeof(IntPropertyTest), _tbr);
        Assert.AreEqual(TestTBR.ExpectedResult.Int, tmm.Wrappers[0].mapping);
    }

    [TestMethod]
    public void MapTestBoolean()
    {
        var tmm = new TypeMembersMapping<TestTBR.ExpectedResult>(typeof(BooleanPropertyTest), _tbr);
        Assert.AreEqual(TestTBR.ExpectedResult.Boolean, tmm.Wrappers[0].mapping);
    }

    [TestMethod]
    public void MapTestChar()
    {
        var tmm = new TypeMembersMapping<TestTBR.ExpectedResult>(typeof(CharPropertyTest), _tbr);
        Assert.AreEqual(TestTBR.ExpectedResult.Char, tmm.Wrappers[0].mapping);
    }

    public class TestTBR : TypeBasedRegistry<TestTBR.ExpectedResult>
    {
        public TestTBR()
        {
            Add(new (Type type, ExpectedResult result)[]
            {
                new(typeof(int), ExpectedResult.Int),
                new(typeof(string), ExpectedResult.String),
                new(typeof(bool), ExpectedResult.Boolean),
                new(typeof(char), ExpectedResult.Char)
            });
        }

        public enum ExpectedResult
        {
            Int, 
            String,
            Boolean,
            Char
        }
    }

    private class StringPropertyTest
    {
        public string TestProperty { get; set; }
    }

    private class IntPropertyTest
    {
        internal int TestProperty { get; set; }
    }

    private class BooleanPropertyTest
    {
        private bool TestProperty { get; set; }
    }

    private class CharPropertyTest
    {
        protected char TestProperty { get; set; }
    }
}