using Docomposer.Utils.Converters;
using NUnit.Framework;

namespace Docomposer.Core.Test.Converters
{
    [TestFixture]
    class TestDocxConverterConvertToHtml
    {
        private const string DocxToConvert = @"LiquidXml/Resources/docx-to-convert.docx";

        [Test]
        public void TestConversionToHtml()
        {
            // https://github.com/opendocx/Open-Xml-PowerTools/tree/vNext/OpenXmlPowerToolsExamples/WmlToHtmlConverter02
            var htmlString = DocxConverter.ConvertToHtml(DocxToConvert);

            StringAssert.Contains("<!DOCTYPE html", htmlString);
            StringAssert.Contains("doc-reuse-", htmlString);
        }
    }
}