using System;
using Docomposer.Core.Api;
using Docomposer.Core.Domain;
using Docomposer.Utils;

namespace Docomposer.Core.Util
{
    public class CoreUtils
    {
        public static void UpdateDocumentFileLastWriteTime(Document document)
        {
            var fileHandler = ThisApp.FileHandler();
            
            if (document != null)
            {
                var documentFile = fileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(), document.ProjectId.ToString(), DirName.Templates, document.Name + ".docx");
                try
                {
                    fileHandler.SetLastWriteTime(documentFile, DateTime.Now);
                    
                    var compositions = DocumentsCompositions.GetCompositionsUsingDocumentByDocumentId(document.Id);
                    foreach (var c in compositions)
                    {
                        UpdateCompositionFileLastWriteTime(c);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }    
            }
        }

        public static void UpdateCompositionFileLastWriteTime(Composition composition)
        {
            var fileHandler = ThisApp.FileHandler();
            
            if (composition != null)
            {
                var compositionFile = fileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(), composition.ProjectId.ToString(), DirName.CompiledCompositions, composition.Name + ".docx");
                try
                {
                    fileHandler.SetLastWriteTime(compositionFile, DateTime.Now);
                }
                catch (Exception)
                {
                    // ignored
                }    
            }
        }
    }
}