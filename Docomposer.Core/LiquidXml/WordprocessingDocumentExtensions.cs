using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docomposer.Core.LiquidXml.Objects;
using Docomposer.Core.LiquidXml.Tags;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Docomposer.Core.LiquidXml
{
    public static class WordprocessingDocumentExtensions
    {
        public static IEnumerable<W.SdtElement> LiquidXmlElements(this WordprocessingDocument wpDocument)
        {
            var document = wpDocument.MainDocumentPart.Document;

            return document.Descendants<W.SdtElement>().Where(e => e.LxIsElement()).ToList();
        }

        public static IEnumerable<W.SdtElement> LiquidXmlElements(this OpenXmlElement element)
        {
            return element.Descendants<W.SdtElement>().Where(e => e.LxIsElement()).ToList();
        }

        public static List<OpenXmlElement> CloneOpenXmlElementList(this IEnumerable<OpenXmlElement> elements)
        {
            var clone = new List<OpenXmlElement>();
            
            foreach (var element in elements)
            {
                clone.Add(element.CloneNode(true));
            }

            return clone;
        }
        
        //AST = Abstract Syntax Tree: https://en.wikipedia.org/wiki/Abstract_syntax_tree
        public static LiquidXmlAst LiquidXmlBuildAst(this WordprocessingDocument wpDocument)
        {
            var elements = wpDocument.LiquidXmlElements();

            var ast = new LiquidXmlAst
            {
                Root = new LiquidXmlElement
                {
                    Parent = null,
                    Descendants = new List<LiquidXmlElement>(),
                    SdtElement = null
                }
            };

            var roots = new List<LiquidXmlElement>
            {
                ast.Root
            };

            foreach (var e in elements)
            {

                if (e.LxIsObject())
                {
                    var el = e.LxToLiquidXmlElement().LxToDerivedLiquidXmlElement();
                    roots.Last().Descendants.Add(el);
                    el.Parent = roots.Last();
                }

                if (e.LxIsBeginForTag())
                {
                    var el = e.LxToLiquidXmlElement().LxToDerivedLiquidXmlElement();
                    roots.Last().Descendants.Add(el);
                    el.Parent = roots.Last();
                    roots.Add(el);
                }

                if (e.LxIsEndForTag())
                {
                    if (roots.Count > 1)
                    {
                        roots.RemoveAt(roots.Count - 1);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid template");
                    }

                    var el = e.LxToLiquidXmlElement().LxToDerivedLiquidXmlElement();
                    roots.Last().Descendants.Add(el);
                    el.Parent = roots.Last();
                }
            }

            if (!ast.IsValid())
            {
                throw new ArgumentException("Invalid template");
            }
            
            if (roots.Count == 1) return ast;

            throw new ArgumentException("Invalid template");
        }

        public static List<string> GetQueryNamesFromDocument(this WordprocessingDocument wpDocument)
        {
            var queryNames = new List<string>();
            var ast = wpDocument.LiquidXmlBuildAst();

            LiquidXmlBeginForTag beginForTag = null;
            
            foreach (var descendant in ast.Root.Descendants)
            {
                var lxElement = descendant.LxToDerivedLiquidXmlElement();

                if (lxElement.GetType() == typeof(LiquidXmlObject))
                {
                    var objectName = ((LiquidXmlObject)lxElement).GetObjectName();

                    if (!queryNames.Contains(objectName))
                    {
                        queryNames.Add(objectName);    
                    }
                }
                
                if (descendant.SdtElement.LxIsBeginForTag())
                {
                    beginForTag = (LiquidXmlBeginForTag) descendant.LxToDerivedLiquidXmlElement();
                }

                if (descendant.SdtElement.LxIsEndForTag() && beginForTag != null)
                {
                    var collectionName = beginForTag.GetObjectCollectionName();

                    if (!queryNames.Contains(collectionName))
                    {
                        queryNames.Add(collectionName);    
                    }
                }
            }

            return queryNames;
        }

        public static void ProcessDocumentWithDataFrom(this WordprocessingDocument wpDocument, List<System.Data.DataTable> tables)
        {
            var ast = wpDocument.LiquidXmlBuildAst();

            LiquidXmlBeginForTag beginForTag = null;

            foreach (var descendant in ast.Root.Descendants)
            {
                var lxElement = descendant.LxToDerivedLiquidXmlElement();

                if (lxElement.GetType() == typeof(LiquidXmlObject) && beginForTag == null)
                {
                    ((LiquidXmlObject)descendant).UpdateWithDataFrom(tables);    
                }
                
                if (descendant.SdtElement.LxIsBeginForTag())
                {
                    beginForTag = (LiquidXmlBeginForTag) descendant.LxToDerivedLiquidXmlElement();
                }

                if (descendant.SdtElement.LxIsEndForTag() && beginForTag != null)
                {
                    var endForTag = (LiquidXmlEndForTag) descendant.LxToDerivedLiquidXmlElement();
                    var forTag = new LiquidXmlForTag(beginForTag, endForTag);
                    var elementsToLoop = forTag.OpenXmlElementsToLoop;
                    var commonAncestor = beginForTag.SdtElement.LxCommonAncestorWith(endForTag.SdtElement);

                    var forTagIndex = 0;

                    if (commonAncestor.GetType() != typeof(W.Table))
                    {
                        for (var i = 0; i < commonAncestor.ChildElements.Count; i++)
                        {
                            if (commonAncestor.ChildElements[i] == forTag.BeginTag.SdtElement)
                            {
                                forTagIndex = i;
                                break;
                            }
                        }    
                    }
                    
                    var objectName = beginForTag.GetObjectName();
                    var collectionName = beginForTag.GetObjectCollectionName();

                    var table = tables.FirstOrDefault(t => t.TableName == collectionName);

                    if (table != null)
                    {
                        var elementsToAppend = new List<OpenXmlElement>();
                        
                        foreach (DataRow row in table.Rows)
                        {
                            var clonedElementsToLoop = ClonedElementsToLoopUpdatedWithData(elementsToLoop, objectName, row);

                            elementsToAppend.AddRange(clonedElementsToLoop);
                        }

                        foreach (var el in elementsToAppend)
                        {
                            if (commonAncestor.GetType() == typeof(W.Table))
                            {
                                commonAncestor.AppendChild(el);
                            }
                            else
                            {
                                commonAncestor.InsertAt(el, forTagIndex);
                            }
                        }
                        
                        forTag.CleanUpTemplateMarkup();
                    }

                    beginForTag = null;
                }
            }
        }

        private static List<OpenXmlElement> ClonedElementsToLoopUpdatedWithData(IList<OpenXmlElement> elementsToLoop, string objectName, DataRow row)
        {
            var clonedElementsToLoop = elementsToLoop.CloneOpenXmlElementList();

            foreach (var clonedElement in clonedElementsToLoop)
            {
                var clonedLxElements = clonedElement.LiquidXmlElements();

                foreach (var clonedLxElement in clonedLxElements)
                {
                    var element = clonedLxElement.LxToLiquidXmlElement().LxToDerivedLiquidXmlElement();

                    if (element.GetType() == typeof(LiquidXmlObject))
                    {
                        var lxObject = (LiquidXmlObject) element;
                        
                        if (lxObject.GetObjectName() == objectName)
                        {
                            var value = row[lxObject.GetObjectProperty()].ToString();
                            
                            W.SdtElement sdtElement = null;
                            if (lxObject.SdtElement.GetType() == typeof(W.SdtCell))
                            {
                                sdtElement = (W.SdtCell) lxObject.SdtElement;    
                            } else if (lxObject.SdtElement.GetType() == typeof(W.SdtRun))
                            {
                                sdtElement = (W.SdtRun) lxObject.SdtElement;
                            }

                            if (sdtElement != null)
                            {
                                var runs = sdtElement.Descendants<W.Run>().ToList();

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

            return clonedElementsToLoop;
        }

        public static OpenXmlElement GetCommonParentFor(this WordprocessingDocument doc, OpenXmlElement first, OpenXmlElement second)
        {
            var elements = doc.MainDocumentPart.Document.Descendants();

            foreach (var e in elements)
            {

            }

            return null;
        }

        private static bool IsValid(this LiquidXmlAst ast)
        {
            var beginForTags = ast.Root.Descendants.Count(d => d.SdtElement.LxIsBeginForTag());
            var endForTags = ast.Root.Descendants.Count(d => d.SdtElement.LxIsEndForTag());

            return beginForTags == endForTags;
        }
    }
}
