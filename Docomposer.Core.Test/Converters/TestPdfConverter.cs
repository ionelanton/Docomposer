using Docomposer.Data.Util;
using Docomposer.Utils;
using NUnit.Framework;

namespace Docomposer.Core.Test.Converters
{
    [TestFixture]
    public class TestPdfConverter
    {
        [Test]
        public void TestDocReusePdfConverter()
        {
            const string inputDocxFileName = "docx-to-convert.docx";
            const string outputPdfFileName = "docx-to-convert.pdf";

            var fileHandler = new FileSystemHandler();

            var baseDirectory = ThisApp.BaseDirectory();
            
            var inputDir = fileHandler.CombinePaths(baseDirectory, "LiquidXml", "Resources");
            var outputDir = fileHandler.CombinePaths(baseDirectory, "Output");

            if (!fileHandler.ExistsPath(outputDir))
            {
                fileHandler.CreatePath(outputDir);
            }

            var inputFile = fileHandler.CombinePaths(inputDir, inputDocxFileName);
            var outputFile = fileHandler.CombinePaths(outputDir, outputPdfFileName);

            var success = ThisApp.PdfConverter().ConvertDocxToPdf(inputFile, outputFile);

            Assert.IsTrue(success);
            Assert.True(fileHandler.ExistsFile(fileHandler.CombinePaths(outputDir, outputPdfFileName)));
        }
    }
}