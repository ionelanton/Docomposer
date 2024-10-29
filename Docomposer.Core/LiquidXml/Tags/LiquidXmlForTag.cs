using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;

namespace Docomposer.Core.LiquidXml.Tags
{
    public class LiquidXmlForTag
    {
        public LiquidXmlBeginForTag BeginTag { get; set; }
        public LiquidXmlEndForTag EndTag { get; set; }

        private OpenXmlElement _commonAncestor;

        public LiquidXmlForTag(LiquidXmlBeginForTag beginTag, LiquidXmlEndForTag endTag)
        {
            BeginTag = beginTag;
            EndTag = endTag;
        }

        public void CleanUpTemplateMarkup()
        {
            _setCommonAncestor();

            if (_commonAncestor == null) return;
            
            foreach (var element in OpenXmlElementsToLoop)
            {
                var parent = element.Parent;
                parent.RemoveChild(element);
            }

            OpenXmlElement beginForElement = BeginTag.SdtElement;

            while (beginForElement.Parent != null)
            {
                if (beginForElement.Parent.Equals(_commonAncestor))
                {
                    _commonAncestor.RemoveChild(beginForElement);
                    break;
                }
                beginForElement = beginForElement.Parent;
                
            }
            
            OpenXmlElement endForElement = EndTag.SdtElement;

            while (endForElement.Parent != null)
            {
                if (endForElement.Parent.Equals(_commonAncestor))
                {
                    _commonAncestor.RemoveChild(endForElement);
                    break;
                }
                endForElement = endForElement.Parent;
            }
        }

        private void _setCommonAncestor()
        {
            if (_commonAncestor != null) return;
            
            var beginTag = BeginTag.SdtElement;
            var endTag = EndTag.SdtElement;
            _commonAncestor = beginTag.LxCommonAncestorWith(endTag);
        }

        public IList<OpenXmlElement> OpenXmlElementsToLoop
        {
            get
            {
                var elementsToLoop = new List<OpenXmlElement>();

                var beginTag = BeginTag.SdtElement;
                var endTag = EndTag.SdtElement;

                _setCommonAncestor();

                if (_commonAncestor == null) return elementsToLoop;
                
                var childrenOfCommonAncestor = _commonAncestor.ChildElements;

                var isInLoop = false;

                foreach (var child in childrenOfCommonAncestor)
                {
                    if (child.Equals(beginTag) || child.Descendants().Contains(beginTag))
                    {
                        isInLoop = true;
                        continue;
                    }

                    if (child.Equals(endTag) || child.Descendants().Contains(endTag))
                    {
                        break;
                    }

                    if (isInLoop)
                    {
                        elementsToLoop.Add(child);
                    }
                }

                return elementsToLoop;
            }
        }
    }
}