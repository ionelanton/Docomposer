using System;
using Docomposer.Core.LiquidXml;
using Docomposer.Core.LiquidXml.Objects;
using DocumentFormat.OpenXml.Wordprocessing;
using NUnit.Framework;

namespace Docomposer.Core.Test.LiquidXml.Objects
{
    [TestFixture]
    class TestLiquidXmlObject
    {
        [Test]
        public void TestConversionFromJson()
        {
            var json = @"{ ""color"" : ""red"" }";

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            Assert.That(obj["color"].ToString(), Is.EqualTo("red"));
        }

        [Test]
        public void TestConversionFromJsonObject()
        {
            var json = @"{""property"": { ""color"" : ""red"" }  }";

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            Assert.That(obj["property"]["color"].ToString(), Is.EqualTo("red"));
        }
        
        [Test]
        public void TestLiquidXmlObjectDerivationFromLiquidXmlElement()
        {
            var obj = new LiquidXmlElement
            {
                SdtElement = new SdtRun
                {
                    InnerXml =
                        @"
<w:p w:rsidRPr=""00206412"" w:rsidR=""00FE6208"" w:rsidRDefault=""00FE6208"" w14:paraId=""1AA63B8C"" w14:textId=""2E4BD658"" xmlns:w14=""http://schemas.microsoft.com/office/word/2010/wordml"">
    <w:r w:rsidRPr=""00206412"">
        <w:t>{{name}}</w:t>
    </w:r>
</w:p>"
                }
            }.LxToDerivedLiquidXmlElement();

            Assert.That(obj.GetType(), Is.EqualTo(typeof(LiquidXmlObject)));
        }

        [Test]
        public void TestLiquidXmlObjectCastToLiquidXmlElement()
        {
            var obj = (LiquidXmlElement) new LiquidXmlObject();

            Assert.That(obj.GetType(), Is.EqualTo(typeof(LiquidXmlObject)));
        }

        [Test]
        public void TestObjectWithoutProperty()
        {
            var obj = new LiquidXmlObject
            {
                SdtElement = new SdtRun
                {
                    InnerXml =
                        @"
<w:p w:rsidRPr=""00206412"" w:rsidR=""00FE6208"" w:rsidRDefault=""00FE6208"" w14:paraId=""1AA63B8C"" w14:textId=""2E4BD658"" xmlns:w14=""http://schemas.microsoft.com/office/word/2010/wordml"">
    <w:r w:rsidRPr=""00206412"">
        <w:t>{{name}}</w:t>
    </w:r>
</w:p>"
                }
            };

            Assert.That(obj.GetObjectName(), Is.EqualTo("{{name}}"));
            Assert.That(obj.GetObjectProperty(), Is.EqualTo("{{name}}"));
        }

        [Test]
        public void TestGetObjectPropertyValueFromDataTable()
        {
            var obj = new LiquidXmlObject
            {
                SdtElement = new SdtRun
                {
                    InnerXml =
                        @"
<w:p w:rsidRPr=""00206412"" w:rsidR=""00FE6208"" w:rsidRDefault=""00FE6208"" w14:paraId=""1AA63B8C"" w14:textId=""2E4BD658"" xmlns:w14=""http://schemas.microsoft.com/office/word/2010/wordml"">
    <w:r w:rsidRPr=""00206412"">
        <w:t>{{person.name}}</w:t>
    </w:r>
</w:p>"
                }
            };

            var dt = new System.Data.DataTable("person");
            dt.Columns.Add("name");
            var row = dt.NewRow();
            row["name"] = "Johnny";
            dt.Rows.Add(row);

            Assert.That(obj.GetObjectName(), Is.EqualTo("person"));
            Assert.That(obj.GetObjectProperty().Contains("name"));
            
            Assert.That(dt.Rows[0][obj.GetObjectProperty()], Is.EqualTo("Johnny"));
        }

        [Test]
        public void TestObjectWithIndexedProperty()
        {
            var obj = new LiquidXmlObject
            {
                SdtElement = new SdtRun
                {
                    InnerXml =
                        @"
<w:p w:rsidRPr=""00206412"" w:rsidR=""00FE6208"" w:rsidRDefault=""00FE6208"" w14:paraId=""1AA63B8C"" w14:textId=""2E4BD658"" xmlns:w14=""http://schemas.microsoft.com/office/word/2010/wordml"">
    <w:r w:rsidRPr=""00206412"">
        <w:t>{{format.person.age}}</w:t>
    </w:r>
</w:p>"
                }
            };

            Assert.That(obj.GetObjectName(), Is.EqualTo("{{format.person.age}}"));
            Assert.That(obj.GetObjectProperty().Contains("{{format.person.age}}"));
        }
    }
}