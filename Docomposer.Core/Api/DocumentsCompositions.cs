using System;
using System.Collections.Generic;
using System.Linq;
using Docomposer.Core.Domain;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using LinqToDB;

namespace Docomposer.Core.Api
{
    public static class DocumentsCompositions
    {
        public static int CreateDocumentComposition(int documentId, int compositionId, int documentCompositionPredecessorId = 0)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var tst = new TableDocumentComposition
                {
                    CompositionId = compositionId,
                    DocumentId = documentId,
                    PredecessorId = documentCompositionPredecessorId
                };
                
                var result = db.InsertWithInt32Identity(tst);
                db.CommitTransaction();
                return result;
            }
        }
        
        public static int UpdateDocumentCompositionPredecessor(int documentCompositionId, int documentCompositionPredecessorId)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var result = db.DocumentComposition.Where(st => st.Id == documentCompositionId).Update(st => new TableDocumentComposition
                {
                    PredecessorId = documentCompositionPredecessorId
                });
                db.CommitTransaction();
                return result;
            }
        }
        
        public static int DeleteDocumentComposition(int documentCompositionId)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var result = db.DocumentComposition.Where(c => c.Id == documentCompositionId).Delete();
                db.CommitTransaction();
                return result;
            }
        }
        
        public static List<DocumentComposition> GetDocumentsCompositionsByCompositionId(int compositionId)
        {
            using (var db = new DocReuseDataConnection())
            {
                var list = (from tc in db.DocumentComposition
                    join t in db.Document on tc.DocumentId equals t.Id 
                    where tc.CompositionId == compositionId
                    select new DocumentComposition
                    {
                        Id = tc.Id,
                        Name = t.Name,
                        CompositionId = tc.CompositionId,
                        DocumentId = t.Id,
                        PredecessorId = tc.PredecessorId
                    }).ToList();

                if (list.Count == 0) return list;
                
                var first = list.FirstOrDefault(st => st.PredecessorId == 0);

                if (first == null)
                {
                    throw new ArgumentException($"Missing first documentComposition for composition {compositionId}");
                }
                
                var orderedList = new List<DocumentComposition> {first};
                while (orderedList.Count < list.Count)
                {
                    var subsequent = list.FirstOrDefault(st => st.PredecessorId == orderedList.Last().Id);
                    if (subsequent != null)
                    {
                        orderedList.Add(subsequent);
                    }

                    if (subsequent == null && orderedList.Count < list.Count)
                    {
                        throw new ArgumentException(
                            $"Broken predecessor link to document in composition {orderedList.Last().Name}. Please delete all documents from composition and start again.");
                    }
                }

                return orderedList;
            }
        }

        public static List<Composition> GetCompositionsUsingDocumentByDocumentId(int templateId)
        {
            using (var db = new DocReuseDataConnection())
            {
                var composition = (from da in db.DocumentComposition
                    join c in db.Composition on da.CompositionId equals c.Id
                    where da.DocumentId == templateId
                    select new Composition
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ProjectId = c.ProjectId
                    }).ToList();
                return composition;
            }
        }
    }
}