using System;
using System.Collections.Generic;
using System.Linq;
using Docomposer.Core.Domain;
using Docomposer.Data.Databases;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using LinqToDB;

namespace Docomposer.Core.Api
{
    public static class SectionsDocuments
    {
        public static SectionDocument GetSectionDocumentById(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                return (from st in db.SectionDocument
                    join s in db.Section on st.SectionId equals s.Id 
                    where st.Id == id
                    select new SectionDocument
                    {
                        Id = st.Id,
                        Name = s.Name,
                        DocumentId = st.DocumentId,
                        SectionId = s.Id,
                        PredecessorId = st.PredecessorId
                    }).FirstOrDefault();
            }
        }
        
        public static int CreateSectionDocument(int sectionId, int templateId, int sectionDocumentPredecessorId = 0)
        {
            using (var db = new DocReuseDataConnection())
            {
                var tst = new TableSectionDocument
                {
                    SectionId = sectionId,
                    DocumentId = templateId,
                    PredecessorId = sectionDocumentPredecessorId
                };
                
                return db.InsertWithInt32Identity(tst);
            }
        }

        public static int UpdateSectionDocumentPredecessor(int sectionTemplateId, int sectionTemplatePredecessorId)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                
                var result = db.SectionDocument.Where(st => st.Id == sectionTemplateId).Update(st => new TableSectionDocument
                {
                    PredecessorId = sectionTemplatePredecessorId
                });

                db.CommitTransaction();
                
                return result;
            }
        }

        public static int DeleteSectionDocument(int sectionTemplateId)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var result = db.SectionDocument.Where(c => c.Id == sectionTemplateId).Delete();
                db.CommitTransaction();
                return result;
            }
        }

        public static List<SectionDocument> GetSectionsDocumentsByDocumentId(int templateId)
        {
            using (var db = new DocReuseDataConnection())
            {
                var list = (from st in db.SectionDocument
                    join s in db.Section on st.SectionId equals s.Id 
                    where st.DocumentId == templateId
                    select new SectionDocument
                    {
                        Id = st.Id,
                        Name = s.Name,
                        DocumentId = st.DocumentId,
                        SectionId = s.Id,
                        PredecessorId = st.PredecessorId
                    }).ToList();

                if (list.Count == 0) return list;
                
                var first = list.FirstOrDefault(st => st.PredecessorId == 0);

                if (first == null)
                {
                    db.SectionDocument.Where(sd => sd.DocumentId == templateId).Set(sd => sd.PredecessorId, 0).Update();
                    throw new ArgumentException($"Broken order in the list of included sections of document template {templateId}");
                }
                
                var orderedList = new List<SectionDocument> {first};
                while (orderedList.Count < list.Count)
                {
                    var subsequent = list.FirstOrDefault(st => st.PredecessorId == orderedList.Last().Id);
                    if (subsequent != null)
                    {
                        orderedList.Add(subsequent);
                    }

                    if (subsequent == null && orderedList.Count < list.Count)
                    {
                        db.SectionDocument.Where(sd => sd.DocumentId == templateId).Set(sd => sd.PredecessorId, 0).Update();
                        throw new ArgumentException(
                            $"Broken order in the list of included sections of document template {templateId}");
                    }
                }

                return orderedList;
            }
        }
    }
}