namespace Docomposer.Core.LiquidXml.Tags
{
    public class LiquidXmlBeginForTag : LiquidXmlElement
    {
        private string[] Parts { get; set; }

        private void InitParts()
        {
            if (Parts != null) return;

            var str = SdtElement.InnerText.ToLower().LxRemoveExtraSpaces().Replace("for ", "").Replace(" in ", " ").Replace("{%", "").Replace("%}", "").ToLower();
            Parts = str.Trim().Split(" ");
        }
        
        public string GetObjectName()
        {
            InitParts();
            return Parts[0];
        }

        public string GetObjectCollectionName()
        {
            InitParts();
            return Parts[1];
        }
    }
}
