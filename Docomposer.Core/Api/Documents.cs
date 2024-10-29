using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docomposer.Core.LiquidXml;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Util;
using Docomposer.Utils;
using DocumentFormat.OpenXml.Packaging;
using LinqToDB;
using Clippit.Word;
using Docomposer.Core.Domain;
using Docomposer.Core.Util;
using Document = Docomposer.Core.Domain.Document;
using Domain_Document = Docomposer.Core.Domain.Document;
using Path = System.IO.Path;

namespace Docomposer.Core.Api
{
    public static class Documents
    {
        private static IFileHandler FileHandler = ThisApp.FileHandler();


        public static string GetDocumentAsPdf(int id, bool refresh = false)
        {
            TableDocument document;

            using (var db = new DocReuseDataConnection())
            {
                document = db.Document.FirstOrDefault(t => t.Id == id);

                if (document != null)
                {
                    var compiledDocumentsRelativePath =
                        FileHandler.CombinePaths(document.ProjectId.ToString(), DirName.CompiledDocuments);
                    var compiledDocumentsAbsolutePath =
                        Path.Combine(ThisApp.DocReuseCacheDirectory(), compiledDocumentsRelativePath);

                    var pdfFile = FileHandler.CombinePaths(compiledDocumentsAbsolutePath, document.Id + ".pdf");

                    var originalDocument = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        document.ProjectId.ToString(),
                        DirName.Templates, document.Name + ".docx");

                    if (FileHandler.ExistsFile(pdfFile) && !refresh)
                    {
                        if (FileHandler.GetLastWriteTimeUtc(pdfFile).Ticks >
                            FileHandler.GetLastWriteTimeUtc(originalDocument).Ticks)
                        {
                            using (var stream = new FileStream(pdfFile, FileMode.Open, FileAccess.Read,
                                       FileShare.ReadWrite))
                            {
                                using (var ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    var base64 = Convert.ToBase64String(ms.ToArray());
                                    return base64;
                                }
                            }
                        }
                    }

                    if (!FileHandler.ExistsPath(compiledDocumentsAbsolutePath))
                    {
                        FileHandler.CreatePath(compiledDocumentsAbsolutePath);
                    }

                    var sources = new List<Source>
                    {
                        new(new WmlDocument(originalDocument))
                    };

                    var sectionsDocuments = SectionsDocuments.GetSectionsDocumentsByDocumentId(id);

                    foreach (var st in sectionsDocuments)
                    {
                        var section = Sections.GetSectionById(st.SectionId);
                        if (section != null)
                        {
                            sources.Add(new Source(new WmlDocument(Path.Combine(ThisApp.DocReuseDocumentsPath(),
                                section.ProjectId.ToString(), DirName.Sections, section.Name + ".docx"))));
                        }
                    }

                    var compiledDocument =
                        FileHandler.CombinePaths(compiledDocumentsAbsolutePath, document.Name + ".docx");
                    DocumentBuilder.BuildDocument(sources.ToList<ISource>(), compiledDocument);

                    if (ThisApp.PdfConverter().ConvertDocxToPdf(compiledDocument, pdfFile))
                    {
                        var compositionsToUpdate = Documents.GetCompositionsHavingDocument(id);
                        foreach (var composition in compositionsToUpdate)
                        {
                            CoreUtils.UpdateCompositionFileLastWriteTime(composition);
                        }

                        using var stream =
                            new FileStream(pdfFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var ms = new MemoryStream();
                        stream.CopyTo(ms);
                        var base64 = Convert.ToBase64String(ms.ToArray());
                        return base64;
                    }
                }
            }

            var doc = document != null ? document.Name : id.ToString();

            throw new ArgumentNullException($"Error converting document {doc} to PDF.");
        }

        public static WmlDocument GenerateWordDocument(int id, List<DataQueryModel> queries)
        {
            TableDocument document;

            using (var db = new DocReuseDataConnection())
            {
                document = db.Document.FirstOrDefault(t => t.Id == id);

                if (document != null)
                {
                    var originalDocument = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        document.ProjectId.ToString(), DirName.Templates, document.Name + ".docx");

                    var sources = new List<Source>();

                    // remove all content of the document template
                    var aByteArray = FileHandler.ReadAllBytes(originalDocument);
                    using (var stream = new MemoryStream())
                    {
                        stream.Write(aByteArray, 0, (int)aByteArray.Length);
                        using (var wd = WordprocessingDocument.Open(stream, true))
                        {
                            //this line removes header content, why ? 
                            //wd.MainDocumentPart.Document.Body.RemoveAllChildren();

                            // This block generates sometimes "Sequence contains no elements" error
                            // in DocumentBuilder.BuildDocument method
                            // ----------------------------------------------------
                            // foreach (var descendant in wd.MainDocumentPart.Document.Body.Descendants())
                            // {
                            //     descendant.Remove();
                            // }

                            wd.Save();
                        }

                        sources.Add(new Source(new WmlDocument(originalDocument, stream)));
                    }

                    //sources.Add( new Source(originalDocument));

                    var sectionsDocuments = SectionsDocuments.GetSectionsDocumentsByDocumentId(id);

                    for (var i = 0; i < sectionsDocuments.Count; i++)
                    {
                        var section = Sections.GetSectionById(sectionsDocuments[i].SectionId);
                        var file = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                            section.ProjectId.ToString(),
                            DirName.Sections, section.Name + ".docx");

                        var copiedQueries = queries.UpdateDataTablesFromQueries();
                       
                        var anotherByteArray = FileHandler.ReadAllBytes(file);
                        using (var stream = new MemoryStream())
                        {
                            stream.Write(anotherByteArray, 0, (int)anotherByteArray.Length);
                            using (var wd = WordprocessingDocument.Open(stream, true))
                            {
                                var tables = copiedQueries.Select(q => q.DataTable).ToList();
                                //todo-image-artefacts: if image container artefacts are presents, check document sources  
                                wd.ProcessDocumentWithDataFrom(tables);
                                wd.Save();
                            }

                            sources.Add(new Source(new WmlDocument(file, stream)));
                        }
                    }

                    return DocumentBuilder.BuildDocument(sources.ToList<ISource>());
                }
            }

            var doc = document != null ? document.Name : id.ToString();

            throw new ArgumentException($"Document {doc} not found");
        }
        
        public static int AddDocument(Domain_Document document)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var documents = (from d in db.Document
                    where d.ProjectId == document.ProjectId
                    select d).ToList();
                if (!documents.Exists(t =>
                        string.Equals(t.Name.Trim(), document.Name.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    var destPath = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        $"{document.ProjectId}/{DirName.Templates}/");

                    if (!FileHandler.ExistsPath(destPath))
                    {
                        FileHandler.CreatePath(destPath);
                    }

                    var documentId = db.InsertWithInt32Identity(document);

                    var destFile = FileHandler.CombinePaths(destPath, $"{document.Name}.docx");

                    FileHandler.Copy(ProcessUtils.EmptyDocxFile(), destFile);
                    db.CommitTransaction();
                    return documentId;
                }

                throw new ArgumentException($"Document {document.Name} already in database");
            }
        }

        public static void UpdateDocument(Domain_Document document)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();

                var documents = (from t in db.Document where t.Id > 0 select t).ToList();

                if (documents.Exists(t => t.ProjectId == document.ProjectId && t.Name == document.Name))
                {
                    throw new DatabaseException($"Document {document.Name} already in database.");
                }

                var documentsInDatabase = documents.FirstOrDefault(t => t.Id == document.Id);

                if (documentsInDatabase != null)
                {
                    var existingFile = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        documentsInDatabase.ProjectId.ToString(), DirName.Templates,
                        documentsInDatabase.Name + ".docx");

                    var newFile = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        documentsInDatabase.ProjectId.ToString(), DirName.Templates, document.Name + ".docx");

                    var fileInUseDetails = FileHandler.IsFileInUse(existingFile);

                    if (!fileInUseDetails.InUse)
                    {
                        try
                        {
                            FileHandler.Move(existingFile, newFile);
                        }
                        catch (Exception ex)
                        {
                            throw new IOException(
                                $"Cannot rename document {documentsInDatabase.Name} to {document.Name}");
                        }
                        
                        var tableDocument = new TableDocument
                        {
                            Id = document.Id,
                            Name = document.Name,
                            ProjectId = document.ProjectId
                        };
                        db.Update(tableDocument);
                        db.CommitTransaction();
                        
                    }
                    else
                    {
                        //var users = string.Join(", ", client.GetUsersLockingResource(existingUri).ToArray());
                        throw new IOException($"Cannot rename section because is locked by {fileInUseDetails.Users}");
                    }
                }
            }
        }

        public static void DeleteDocument(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var document = (from t in db.Document
                    where t.Id == id
                    select t).FirstOrDefault();

                if (document != null)
                {
                    var sectionsCount = (from s in db.SectionDocument
                        where s.DocumentId == document.Id
                        select s).Count();

                    if (sectionsCount > 0)
                    {
                        throw new ArgumentException($"The document {document.Name} has sections.");
                    }

                    var file = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(), document.ProjectId.ToString(), DirName.Templates, document.Name + ".docx");

                    var fileInUseDetails = FileHandler.IsFileInUse(file);
                    
                    if (!fileInUseDetails.InUse)
                    {
                        try
                        {
                            FileHandler.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Cannot delete document {document.Name}");
                        }
                        
                        db.Document.Where(t => t.Id == id).Delete();

                        var cachedPdf = System.IO.Path.Combine(ThisApp.DocReuseCacheDirectory(), document.ProjectId.ToString(),
                            DirName.CompiledDocuments, document.Id + ".pdf");
                        FileHandler.Delete(cachedPdf);
                        var cachedDocx = System.IO.Path.Combine(ThisApp.DocReuseCacheDirectory(), document.ProjectId.ToString(),
                            DirName.CompiledDocuments, document.Name + ".docx");
                        
                        File.Delete(cachedDocx);

                        db.CommitTransaction();
                    }
                    else
                    {
                        //var users = string.Join(", ", client.GetUsersLockingResource(file).ToArray());
                        var users = fileInUseDetails.Users;
                        throw new IOException($"Cannot delete document because is locked by {users}");
                    }
                }
            }
        }

        public static Domain_Document GetDocumentById(int documentId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from t in db.Document
                    where t.Id == documentId
                    select new Domain_Document
                    {
                        Id = t.Id,
                        Name = t.Name,
                        ProjectId = t.ProjectId
                    }).FirstOrDefault();
            }
        }

        public static List<Domain_Document> GetDocumentsByProjectId(int projectId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from t in db.Document
                    where t.ProjectId == projectId
                    select new Domain_Document
                    {
                        Id = t.Id,
                        Name = t.Name,
                        ProjectId = t.ProjectId
                    }).ToList();
            }
        }

        public static List<Composition> GetCompositionsHavingDocument(int templateId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from tc in db.DocumentComposition
                    join c in db.Composition on tc.CompositionId equals c.Id
                    where tc.DocumentId == templateId
                    select new Composition
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ProjectId = c.ProjectId
                    }).ToList();
            }
        }

        public static List<Domain_Document> GetDocuments()
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from d in db.Document
                    select new Domain_Document
                    {
                        Id = d.Id,
                        Name = d.Name,
                        ProjectId = d.ProjectId
                    }).ToList();
            }
        }
    }
}