using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Util;
using Docomposer.Utils;
using LinqToDB;
using Clippit.Word;
using Docomposer.Core.Domain;
using Docomposer.Core.Util;

namespace Docomposer.Core.Api
{
    public static class Compositions
    {
        public static string GetCompositionAsPdf(int id, bool refresh = false)
        {
            using var db = new DocReuseDataConnection();

            var composition = db.Composition.FirstOrDefault(c => c.Id == id);

            if (composition != null)
            {
                var compiledCompositionsRelativePath =
                    Path.Combine(composition.ProjectId.ToString(), DirName.CompiledCompositions);
                var compiledCompositionsAbsolutePath =
                    Path.Combine(ThisApp.DocReuseCacheDirectory(), compiledCompositionsRelativePath);

                var pdfFile = Path.Combine(compiledCompositionsAbsolutePath, composition.Id + ".pdf");
                var compiledComposition = Path.Combine(compiledCompositionsAbsolutePath, composition.Name + ".docx");

                if (File.Exists(pdfFile) && !refresh)
                {
                    if (File.GetLastWriteTimeUtc(pdfFile).Ticks >= File.GetLastWriteTimeUtc(compiledComposition).Ticks)
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

                if (!Directory.Exists(compiledCompositionsAbsolutePath))
                {
                    Directory.CreateDirectory(compiledCompositionsAbsolutePath);
                }

                var documentsCompositions = DocumentsCompositions.GetDocumentsCompositionsByCompositionId(id);

                var sources = documentsCompositions.Select(documentComposition => new Source(Documents.GenerateWordDocument(documentComposition.DocumentId, new List<DataQueryModel>()), true)).ToList<ISource>();

                // Validate documents -->>                
                var i = 0;
                foreach (var source in sources)
                {
                    var barr = source.WmlDocument.DocumentByteArray;
                    using (var stream = new MemoryStream(0))
                    {
                        stream.Write(barr, 0, (int)barr.Length);
                        var errors = DocumentUtils.ValidateWordDocument(stream, documentsCompositions[i].Name);
                        //todo: write validation errors to log file
                        Console.Write(errors);
                    }
                    i++;
                }
                // <<-- Validate documents
                
                // Add page break
                var pageBreakFile = Path.Combine(ThisApp.BaseDirectory(), DirName.Util,
                    DirName.Resources, "pagebreak.docx");
                var pageBreak = new WmlDocument(pageBreakFile);
                
                var limit = sources.Count;
                var count = 0;
                for (var j = 1; j < limit; j++)
                {
                    sources.Insert(j + count, new Source(pageBreak));
                    count++;
                }
                
                DocumentBuilder.BuildDocument(sources, compiledComposition);

                if (ThisApp.PdfConverter().ConvertDocxToPdf(compiledComposition, pdfFile))
                {
                    using var stream =
                        new FileStream(pdfFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    var base64 = Convert.ToBase64String(ms.ToArray());
                    return base64;
                }
            }

            throw new ArgumentNullException($"Error converting composition {id} to PDF.");
        }

        public static WmlDocument GenerateWordDocument(int id, List<DataQueryModel> queries)
        {
            using var db = new DocReuseDataConnection();
            var composition = db.Composition.FirstOrDefault(c => c.Id == id);

            if (composition != null)
            {
                var documentsComposition = DocumentsCompositions.GetDocumentsCompositionsByCompositionId(id);
                
                var sources = documentsComposition.Select(documentComposition => new Source(Documents.GenerateWordDocument(documentComposition.DocumentId, queries), true)).ToList();
                
                var pageBreakFile = Path.Combine(ThisApp.BaseDirectory(), DirName.Util,
                    DirName.Resources, "pagebreak.docx");
                var pageBreak = new WmlDocument(pageBreakFile);
                
                var limit = sources.Count;
                var count = 0;
                for (var i = 1; i < limit; i++)
                {
                    sources.Insert(i + count, new Source(pageBreak));
                    count++;
                }

                return DocumentBuilder.BuildDocument(sources.ToList<ISource>());
            }

            throw new ArgumentException($"Composition having ID {id} not found");
        }

        public static int AddComposition(Composition composition)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var compositions = (from d in db.Composition
                    where d.ProjectId == composition.ProjectId
                    select d).ToList();
                if (!compositions.Exists(c =>
                        string.Equals(c.Name.Trim(), composition.Name.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    var result = db.InsertWithInt32Identity(composition);
                    db.CommitTransaction();
                    return result;
                }

                db.RollbackTransaction();
                throw new ArgumentException($"Composition {composition.Name} already in database");
            }
        }

        public static void UpdateComposition(Composition composition)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var compositions = (from a in db.Composition where a.Id > 0 select a).ToList();

                if (compositions.Exists(c => c.ProjectId == composition.ProjectId && c.Name == composition.Name))
                {
                    throw new DatabaseException($"Composition {composition.Name} already in database.");
                }

                var documentsInDatabase = compositions.FirstOrDefault(t => t.Id == composition.Id);

                if (documentsInDatabase != null)
                {
                    db.Update(composition);
                    db.CommitTransaction();
                    //todo: update compiled folder?
                }
            }
        }

        public static void DeleteComposition(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var folder = (from t in db.Composition
                    where t.Id == id
                    select t).FirstOrDefault();

                if (folder != null)
                {
                    var templatesCount = (from t in db.DocumentComposition
                        where t.CompositionId == folder.Id
                        select t).Count();

                    if (templatesCount > 0)
                    {
                        throw new ArgumentException($"The composition {folder.Name} has documents.");
                    }

                    db.Composition.Where(t => t.Id == id).Delete();
                    db.CommitTransaction();
                }
            }
        }

        public static Composition GetCompositionById(int compositionId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from c in db.Composition
                    where c.Id == compositionId
                    select new Composition
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ProjectId = c.ProjectId
                    }).FirstOrDefault();
            }
        }

        public static List<Composition> GetCompositions()
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from a in db.Composition
                    select new Composition
                    {
                        Id = a.Id,
                        Name = a.Name,
                        ProjectId = a.ProjectId
                    }).ToList();
            }
        }

        public static List<Composition> GetCompositionsByProjectId(int projectId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from a in db.Composition
                    where a.ProjectId == projectId
                    select new Composition
                    {
                        Id = a.Id,
                        Name = a.Name,
                        ProjectId = a.ProjectId
                    }).ToList();
            }
        }
    }
}