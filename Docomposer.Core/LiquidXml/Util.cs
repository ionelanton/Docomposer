using DocumentFormat.OpenXml;

namespace Docomposer.Core.LiquidXml
{
    public static class Util
    {
        public static string LxRemoveExtraSpaces(this string str)
        {
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }

            return str;
        }

        public static string LxRemoveAllSpaces(this string str)
        {
            return str.Replace(" ", "");
        }

        public static OpenXmlElement LxCommonAncestorWith(this OpenXmlElement el1, OpenXmlElement el2)
        {
            foreach (var a1 in el1.Ancestors())
            {
                foreach (var a2 in el2.Ancestors())
                {
                    if (a1.Equals(a2))
                    {
                        return a1;
                    }
                }
            }

            return null;
        }
    }
}
