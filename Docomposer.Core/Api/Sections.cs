using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Docomposer.Core.Domain;
using Docomposer.Core.Util;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Util;
using Docomposer.Utils;
using LinqToDB;

namespace Docomposer.Core.Api
{
    public static class Sections
    {
        private static readonly IFileHandler FileHandler = ThisApp.FileHandler();

        public static string GetSectionAsPdf(int id, bool refresh = false)
        {
            using (var db = new DocReuseDataConnection())
            {
                var section = db.Section.FirstOrDefault(s => s.Id == id);

                if (section != null)
                {
                    var compiledSectionsRelativePath = FileHandler.CombinePaths(section.ProjectId.ToString(), DirName.CompiledSections);
                    var compiledSectionsAbsolutePath = Path.Combine(ThisApp.DocReuseCacheDirectory(), compiledSectionsRelativePath);

                    var pdfFile = FileHandler.CombinePaths(compiledSectionsAbsolutePath, section.Id + ".pdf");
                    
                    var originalSection = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        section.ProjectId.ToString(),
                        DirName.Sections, section.Name + ".docx");
                    
                    if (FileHandler.ExistsFile(pdfFile) && !refresh)
                    {
                        //Use the cache if docx not changed
                        var docxTime = FileHandler.GetLastWriteTimeUtc(originalSection).Ticks;
                        var pdfTime = FileHandler.GetLastWriteTimeUtc(pdfFile).Ticks;

                        if (docxTime < pdfTime)
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
                    
                    if (!FileHandler.ExistsPath(compiledSectionsAbsolutePath))
                    {
                        FileHandler.CreatePath(compiledSectionsAbsolutePath);
                    }
                    
                    var outputPdfFile = FileHandler.CombinePaths(compiledSectionsAbsolutePath, $"{section.Id}.pdf");
            
                    var documentsToUpdate = GetDocumentsHavingSection(section.Id);

                    foreach (var document in documentsToUpdate)
                    {
                        CoreUtils.UpdateDocumentFileLastWriteTime(document);
                        var compositionsToUpdate = Documents.GetCompositionsHavingDocument(document.Id);
                        foreach (var composition in compositionsToUpdate)
                        {
                            CoreUtils.UpdateCompositionFileLastWriteTime(composition);
                        }
                    }
            
                    var pdfFileName = ThisApp.PdfConverter().ConvertDocxToPdf(originalSection, outputPdfFile) ? outputPdfFile : string.Empty;
                    
                    if (pdfFileName.Length > 0)
                    {
                        using var stream = new FileStream(pdfFileName, FileMode.Open, FileAccess.Read,
                            FileShare.ReadWrite);
                        
                        using var ms = new MemoryStream();
                        stream.CopyTo(ms);
                        var base64 = Convert.ToBase64String(ms.ToArray());
                        return base64;
                    }
                }
            }

            throw new ArgumentNullException($"Error converting section {id} to PDF.");
        }

        public static Section GetSectionById(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                var tableSection = db.Section.FirstOrDefault(s => s.Id == id);
                if (tableSection != null)
                {
                    return new Section
                    {
                        Id = id,
                        Name = tableSection.Name,
                        ProjectId = tableSection.ProjectId
                    };
                }

                return null;
            }
        }

        public static int AddSection(Section section)
        {
            using (var db = new DocReuseDataConnection())
            {
                var sections = (from s in db.Section
                    where s.ProjectId == section.ProjectId
                    select s).ToList();

                if (!sections.Exists(s =>
                        string.Equals(s.Name.Trim(), section.Name.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    var destPath = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        section.ProjectId.ToString(), DirName.Sections);

                    if (!FileHandler.ExistsPath(destPath))
                    {
                        FileHandler.CreatePath(destPath);
                    }

                    var sectionId = db.InsertWithInt32Identity(section);

                    var destFile = FileHandler.CombinePaths(destPath, section.Name + ".docx");

                    FileHandler.Copy(ProcessUtils.EmptyDocxFile(), destFile);

                    return sectionId;
                }

                throw new DatabaseException($"Section {section.Name} already in database");
            }
        }

        public static void UpdateSection(Section section)
        {
            using (var db = new DocReuseDataConnection())
            {
                var sections = (from s in db.Section
                    where s.Id > 0
                    select s).ToList();

                if (sections.Exists(s => s.ProjectId == section.ProjectId && s.Name == section.Name))
                {
                    throw new DatabaseException($"Section {section.Name} already in database.");
                }

                var sectionInDatabase = sections.FirstOrDefault(s => s.Id == section.Id);

                if (sectionInDatabase != null)
                {
                    var existingFile = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        sectionInDatabase.ProjectId.ToString(), DirName.Sections, sectionInDatabase.Name + ".docx");

                    var newFile = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(),
                        sectionInDatabase.ProjectId.ToString(), DirName.Sections, section.Name + ".docx");

                    var useDetails = FileHandler.IsFileInUse(existingFile);

                    if (!useDetails.InUse)
                    {
                        FileHandler.Move(existingFile, newFile);

                        var tableSection = new TableSection
                        {
                            Id = section.Id,
                            Name = section.Name,
                            ProjectId = section.ProjectId
                        };
                        db.Update(tableSection);
                    }
                    else
                    {
                        throw new IOException($"Cannot rename section because is opened by {useDetails.Users}");
                    }
                }
            }
        }

        public static void DeleteSection(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                var section = (from s in db.Section
                    where s.Id == id
                    select s).FirstOrDefault();

                if (section != null)
                {
                    var file = FileHandler.CombinePaths(ThisApp.DocReuseDocumentsPath(), section.ProjectId.ToString(),
                        DirName.Sections, section.Name + ".docx");

                    var useDetails = FileHandler.IsFileInUse(file);

                    if (!useDetails.InUse)
                    {
                        FileHandler.Delete(file);
                        
                        var compiledSectionsRelativePath = FileHandler.CombinePaths(section.ProjectId.ToString(), DirName.CompiledSections);
                        var compiledSectionsAbsolutePath = Path.Combine(ThisApp.DocReuseCacheDirectory(), compiledSectionsRelativePath);

                        var pdfFile = FileHandler.CombinePaths(compiledSectionsAbsolutePath, section.Id + ".pdf");

                        if (FileHandler.ExistsFile(pdfFile))
                        {
                            FileHandler.Delete(pdfFile);
                        }

                        db.Section.Where(s => s.Id == id).Delete();
                    }
                    else
                    {
                        throw new IOException($"Cannot delete section because is opened by {useDetails.Users}");
                    }
                }
            }
        }

        public static List<Section> GetSectionsByProjectId(int projectId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from s in db.Section
                    where s.ProjectId == projectId
                    select new Section
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ProjectId = s.ProjectId
                    }).ToList();
            }
        }

        public static List<Document> GetDocumentsHavingSection(int sectionId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from st in db.SectionDocument
                    join t in db.Document on st.DocumentId equals t.Id
                    where st.SectionId == sectionId
                    select new Document
                    {
                        Id = t.Id,
                        Name = t.Name,
                        ProjectId = t.ProjectId
                    }).ToList();
            }
        }
    }
}