using System;
using Docomposer.Data.Util;
using Docomposer.Data.Util.Localization;
using NUnit.Framework;

namespace Docomposer.Data.Test.Util
{
    [TestFixture]
    public class TestLocalization
    {
        [Test]
        public void TestHello()
        {
            Assert.That(nameof(Localization.Settings.ExcelPath), Is.EqualTo("ExcelPath"));
        }
    }
}