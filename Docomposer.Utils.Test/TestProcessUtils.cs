using NUnit.Framework;

namespace Docomposer.Utils.Test
{
    [TestFixture]
    public class TestProcessUtils
    {
        [Test]
        public void TestWordExeFilePath()
        {
            Assert.DoesNotThrow(() => { ProcessUtils.WordExeFilePath(); }, "Microsoft Word not found!");
        }
        
        [Test]
        public void TestExcelExeFilePath()
        {
            Assert.DoesNotThrow(() => { ProcessUtils.ExcelExeFilePath(); }, "Microsoft Word not found!");
        }
    }
}