using System.Collections.Generic;
using Docomposer.Core.LiquidXml.Objects;
using Docomposer.Core.LiquidXml.Tags;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Docomposer.Core.LiquidXml
{
    public class LiquidXmlElement
    {
        public LiquidXmlElement Parent { get; set; }
        public List<LiquidXmlElement> Descendants { get; set; }
        public SdtElement SdtElement { get; set; }

        public bool IsRoot()
        {
            return Parent == null;
        }
    }

    public static class LiquidXmlElementExtensions
    {
        public static LiquidXmlElement LxToDerivedLiquidXmlElement(this LiquidXmlElement lxe)
        {
            if (lxe.SdtElement.LxIsBeginForTag())
            {
                return new LiquidXmlBeginForTag
                {
                    Descendants = lxe.Descendants,
                    SdtElement = lxe.SdtElement,
                    Parent = lxe.Parent
                };
            }

            if (lxe.SdtElement.LxIsEndForTag())
            {
                return new LiquidXmlEndForTag
                {
                    Descendants = lxe.Descendants,
                    SdtElement = lxe.SdtElement,
                    Parent = lxe.Parent
                };
            }

            if (lxe.SdtElement.LxIsObject())
            {
                return new LiquidXmlObject
                {
                    Descendants = lxe.Descendants,
                    SdtElement = lxe.SdtElement,
                    Parent = lxe.Parent
                };
            }


            return null;
        }
    }
}