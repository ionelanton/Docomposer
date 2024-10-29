using Docomposer.Core.LiquidXml;
using NUnit.Framework;

namespace Docomposer.Core.Test.LiquidXml
{
    [TestFixture]
    public class TestUtil
    {
        [Test]
        public void TestLxRemoveAllSpaces()
        {
            var strWithSpaces = "  a     b c                d       ";
            var strWithoutSpaces = strWithSpaces.LxRemoveAllSpaces();
            
            Assert.That(strWithoutSpaces, Is.EqualTo("abcd"));
        }
        
        [Test]
        public void TestLxRemoveExtraSpaces()
        {
            var strWithSpaces = "   a     bc                d       ";
            var strWithoutExtraSpaces = strWithSpaces.LxRemoveExtraSpaces();
            
            Assert.That(strWithoutExtraSpaces, Is.EqualTo(" a bc d "));
        }
    }
}