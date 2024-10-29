using System.Collections.Generic;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Docomposer.Core.LiquidXml.Objects
{
    public class LiquidXmlObject : LiquidXmlElement
    {
        public string GetObjectName()
        {
            var str = SdtElement.InnerText.Replace("{{", "").Replace("}}", "").LxRemoveAllSpaces();

            return str.Count(c => c == '.') == 1 ? str.Split(".")[0] : SdtElement.InnerText;
        }

        public string GetObjectProperty()
        {
            var str = SdtElement.InnerText.Replace("{{", "").Replace("}}", "").LxRemoveAllSpaces();

            return str.Count(c => c == '.') == 1 ? str.Split(".")[1] : SdtElement.InnerText;
        }

        public void UpdateWithDataFrom(IEnumerable<DataTable> tables)
        {
            var tableName = GetObjectName();
            var table = tables.FirstOrDefault(t => t.TableName == tableName);

            if (table != null && table.Rows.Count > 0)
            {
                var prop = GetObjectProperty();
                var value = table.Rows[0][prop].ToString();

                //todo: Clarify SdtContentRun (plain text control) - SdtContentBlock (rich text control)
                
                var element = SdtElement.Descendants<W.SdtContentRun>().FirstOrDefault() ?? (OpenXmlCompositeElement)SdtElement.Descendants<W.SdtContentBlock>().FirstOrDefault();

                if (element != null && element.InnerText.Contains(tableName + "." + prop))
                {
                    var runs = element.Descendants<W.Run>().ToList();

                    foreach (var texts in runs)
                    {
                        var text = texts.Descendants<W.Text>().FirstOrDefault();
                        if (text != null)
                        {
                            text.Text = "";
                        }
                    }

                    foreach (var texts in runs)
                    {
                        var text = texts.Descendants<W.Text>().FirstOrDefault();
                        if (text != null)
                        {
                            text.Text = value;
                        }

                        break;
                    }
                }
            }
        }
    }
}