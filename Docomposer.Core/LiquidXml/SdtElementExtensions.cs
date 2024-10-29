using System.Collections.Generic;
using System.Text.RegularExpressions;
using Docomposer.Core.LiquidXml.Objects;
using Docomposer.Core.LiquidXml.Tags;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using NPOI.SS.Formula.Functions;

namespace Docomposer.Core.LiquidXml
{
    public static class SdtElementExtensions
    {
        public static bool LxIsTag(this SdtElement sdtElement)
        {
            return sdtElement.InnerText.Contains("{%") && sdtElement.InnerText.Contains("%}");
        }

        public static bool LxIsBeginForTag(this SdtElement sdtElement)
        {
            var tag = sdtElement.InnerText.ToLower().LxRemoveExtraSpaces();

            return sdtElement.LxIsTag() &&
                   Regex.Match(tag, @"^[ ]*{%[ ]+for[ ]+\w+[ ]+in[ ]+\w+[ ]+%}[ ]*$").Success;
        }

        public static bool LxIsEndForTag(this SdtElement sdtElement)
        {
            var def = sdtElement.InnerText.ToLower().LxRemoveExtraSpaces();

            return sdtElement.LxIsTag() && Regex.Match(def, @"^[ ]*{%[ ]+endfor[ ]+%}[ ]*$").Success;
        }

        public static bool LxIsObject(this SdtElement sdtElement)
        {
            return sdtElement.InnerText.Contains("{{") && sdtElement.InnerText.Contains("}}");
        }

        public static bool LxIsElement(this SdtElement sdtElement)
        {
            return sdtElement.LxIsTag() || sdtElement.LxIsObject();
        }

        public static OpenXmlElement LxAncestorsOfOpenXmlElement(this SdtElement sdtElement)
        {

            return null;
        }

        public static LiquidXmlElement LxToLiquidXmlElement(this SdtElement sdtElement)
        {
            return new LiquidXmlElement
            {
                Parent = null,
                Descendants = new List<LiquidXmlElement>(),
                SdtElement = sdtElement
            };
        }
    }
}
