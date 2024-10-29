using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docomposer.Core.LiquidXml;
using Docomposer.Core.LiquidXml.Objects;
using Docomposer.Core.LiquidXml.Tags;
using Docomposer.Utils;
using DocumentFormat.OpenXml.Packaging;
using NUnit.Framework;

using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Docomposer.Core.Test.LiquidXml
{
    [TestFixture]
    class TestLiquidXml
    {
        private const string Document01 = @"LiquidXml/Resources/document_01.docx";
        private const string Document02 = @"LiquidXml/Resources/document_02.docx";


        [Test]
        public void TestOpenXmlElementsListFromDocument()
        {
            using (var wd = WordprocessingDocument.Open(Document01, true))
            {
                var elements = wd.MainDocumentPart.Document.Descendants();

                Assert.That(elements.Count, Is.EqualTo(813));
            }
        }

        [Test]
        public void TestGetPlainTextContentControls()
        {
            using (var wd = WordprocessingDocument.Open(Document01, true))
            {
                var document = wd.MainDocumentPart.Document;

                var texts = document.Descendants<W.SdtElement>().ToList();

                Assert.That(texts.Count, Is.EqualTo(13));

                Assert.That(texts[0].InnerText, Is.EqualTo("{{person.firstname}}"));
                Assert.That(texts[1].InnerText, Is.EqualTo("{{person.lastname}}"));
                Assert.That(texts[2].InnerText, Is.EqualTo("{{person.salary}}"));
            }
        }

        [Test]
        public void TestLiquidXmlForTag()
        {
            using (var wd = WordprocessingDocument.Open(Document02, true))
            {
                var document = wd.MainDocumentPart.Document;

                var textControls = document.Descendants<W.SdtElement>().Where(e => e.LxIsElement()).ToList();

                Assert.That(textControls.Count, Is.EqualTo(7));

                foreach (var textControl in textControls)
                {
                    if (textControl.LxIsBeginForTag())
                    {
                        var lxElement = new LiquidXmlElement
                        {
                            SdtElement = textControl,
                        };

                        var lxBeginForTag = (LiquidXmlBeginForTag) lxElement.LxToDerivedLiquidXmlElement();

                        Assert.That(lxBeginForTag.GetObjectName(), Is.EqualTo("product"));
                        Assert.That(lxBeginForTag.GetObjectCollectionName(), Is.EqualTo("products"));
                    }
                }

                Assert.That(textControls.Exists(tag => tag.LxIsEndForTag()));
            }
        }

        [Test]
        public void TestLiquidXmlAstTypes()
        {
            using (var wd = WordprocessingDocument.Open(Document02, true))
            {
                var ast = wd.LiquidXmlBuildAst();

                Assert.That(ast.Root.Descendants[0].LxToDerivedLiquidXmlElement(), Is.TypeOf<LiquidXmlObject>());
                Assert.That(ast.Root.Descendants[1].LxToDerivedLiquidXmlElement(), Is.TypeOf<LiquidXmlObject>());
                Assert.That(ast.Root.Descendants[2].LxToDerivedLiquidXmlElement(), Is.TypeOf<LiquidXmlBeginForTag>());
                Assert.That(ast.Root.Descendants[3].LxToDerivedLiquidXmlElement(), Is.TypeOf<LiquidXmlEndForTag>());
            }
        }

        [Test]
        public void TestLiquidXmlBuildAst()
        {
            using (var wd = WordprocessingDocument.Open(Document02, true))
            {
                var ast = wd.LiquidXmlBuildAst();

                Assert.That(ast.Root.Descendants.Count, Is.EqualTo(4));
                Assert.That(ast.Root.Descendants[2].Descendants.Count, Is.EqualTo(3));

                var productName = ast.Root.Descendants[2].Descendants[0].SdtElement.InnerText;
                Assert.That(productName, Is.EqualTo("{{product.name}}"));
            }
        }
 
        [Test]
        public void TestLiquidXmlCommonAncestor()
        {
            using (var wd = WordprocessingDocument.Open(Document02, true))
            {
                var ast = wd.LiquidXmlBuildAst();

                var beginFor = ast.Root.Descendants[2].SdtElement;
                var endFor = ast.Root.Descendants[3].SdtElement;

                var commonAncestor = beginFor.LxCommonAncestorWith(endFor);

                Assert.That(commonAncestor, Is.Not.Null);
                Assert.That(commonAncestor, Is.TypeOf<W.Table>());
            }
        }

        [Test]
        public void TestLiquidXmlOpenXmlElementsToRepeatInForTag()
        {
            using (var wd = WordprocessingDocument.Open(Document02, true))
            {
                var ast = wd.LiquidXmlBuildAst();

                LiquidXmlBeginForTag beginForTag = null;
                LiquidXmlEndForTag endForTag = null;

                foreach (var element in ast.Root.Descendants)
                {
                    if (element.SdtElement.LxIsBeginForTag())
                    {
                        beginForTag = (LiquidXmlBeginForTag) element;
                    }

                    if (element.SdtElement.LxIsEndForTag())
                    {
                        endForTag = (LiquidXmlEndForTag) element;
                    }
                }

                Assert.That(beginForTag, Is.Not.Null);
                Assert.That(endForTag, Is.Not.Null);

                var forTag = new LiquidXmlForTag(beginForTag, endForTag);

                var elementsToLoop = forTag.OpenXmlElementsToLoop;

                var text = elementsToLoop.First().InnerText;

                StringAssert.Contains("{{product.name}}", text);
                StringAssert.Contains("{{product.price}}", text);
                StringAssert.Contains("{{product.quantity}}", text);
            }
        }

        [Test]
        public void TestGetQueryNamesFromDocument()
        {
            using (var wd = WordprocessingDocument.Open(Document02, true))
            {
                var queryNames = wd.GetQueryNamesFromDocument();
                
                Assert.That(queryNames.Count, Is.EqualTo(2));
                
                Assert.That(queryNames[0], Is.EqualTo("business"));
                Assert.That(queryNames[1], Is.EqualTo("products"));
            }
        }

        [Test]
        public void TestLiquidXmlCompileDocumentWithData()
        {
            var tables = new List<System.Data.DataTable>();
            
            var dt = new System.Data.DataTable("person");
            dt.Columns.Add("firstname");
            dt.Columns.Add("lastname");
            dt.Columns.Add("salary");
            var row = dt.NewRow();
            row["firstname"] = "Johnny";
            row["lastname"] = "Mnemonic";
            row["salary"] = "$250000";
            dt.Rows.Add(row);
            tables.Add(dt);
            
            var dt2 = new System.Data.DataTable("products");
            dt2.Columns.Add("name");
            dt2.Columns.Add("price");
            dt2.Columns.Add("quantity");
            var row2 = dt2.NewRow();
            row2["name"] = "Apples";
            row2["price"] = "1$";
            row2["quantity"] = "12";
            dt2.Rows.Add(row2);
            var row3 = dt2.NewRow();
            row3["name"] = "Oranges";
            row3["price"] = "2$";
            row3["quantity"] = "14";
            dt2.Rows.Add(row3);
            var row4 = dt2.NewRow();
            row4["name"] = "Strawberries";
            row4["price"] = "3$";
            row4["quantity"] = "150";
            dt2.Rows.Add(row4);
            tables.Add(dt2);
            
            using (var stream = new MemoryStream(File.ReadAllBytes(Document01)))
            {
                using (var wd = WordprocessingDocument.Open(stream, true))
                {
                    wd.ProcessDocumentWithDataFrom(tables);
                    wd.Save();

                    using (var sr = new StreamReader(wd.MainDocumentPart.GetStream()))
                    {
                        string content = sr.ReadToEnd();
                        
                        Assert.That(content.Contains("Johnny"));
                        Assert.That(content.Contains("Mnemonic"));
                        Assert.That(content.Contains("$250000"));
                        
                        Assert.That(content.Contains("{%"), Is.False);
                        Assert.That(content.Contains("%}"), Is.False);
                        Assert.That(content.Contains("{{"), Is.False);
                        Assert.That(content.Contains("}}"), Is.False);
                        
                        Assert.That(content.Contains("Apples"));
                        Assert.That(content.Contains("Oranges"));
                        Assert.That(content.Contains("Strawberries"));
                        
                        Assert.That(content.Contains("1$"));
                        Assert.That(content.Contains("2$"));
                        Assert.That(content.Contains("3$"));
                    }

                    var tempPath = Path.GetTempPath();
                    var tempFile = ThisApp.FileHandler().CombinePaths(tempPath, "document.docx");
                    
                    File.WriteAllBytes(tempFile, stream.ToArray());
                    
                    Assert.That(ThisApp.FileHandler().ExistsFile(tempFile));
                }                
            }
        }
    }
}