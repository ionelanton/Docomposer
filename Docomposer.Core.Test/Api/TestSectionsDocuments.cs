using Docomposer.Data.Databases;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Databases.DataStore.Util;
using Docomposer.Data.Databases.Util;
using Docomposer.Data.Test.Database;
using LinqToDB;
using NUnit.Framework;

namespace Docomposer.Core.Test.Api
{
    [TestFixture]
    public class TestSectionsDocuments : TestDatabase
    {
        private DatabaseMigrator _migrator;
        private int _documentId1, _documentId3, _documentId2;
        
        [SetUp]
        public void SetUp()
        {
            _migrator = new DatabaseMigrator();
            _migrator.CleanUp();
            _migrator.Execute();

            using (var db = new DocReuseDataConnection())
            {
                var projectId = db.InsertWithInt32Identity(new TableProject
                {
                    Name = "Project"
                });
                
                _documentId1 = db.InsertWithInt32Identity(new TableDocument
                {
                    Name = "Document", ProjectId = projectId 
                });

                var sectionId1 = db.InsertWithInt32Identity(new TableSection
                {
                    Name = "Section 1", ProjectId = projectId
                });
                
                _documentId2 = db.InsertWithInt32Identity(new TableSection
                {
                    Name = "Section 2", ProjectId = projectId
                });
                
                _documentId3 = db.InsertWithInt32Identity(new TableSection
                {
                    Name = "Section 3", ProjectId = projectId
                });

                db.Insert(new TableSectionDocument
                {
                    PredecessorId = _documentId2,
                    SectionId = sectionId1,
                    DocumentId = _documentId1
                });
                
                db.Insert(new TableSectionDocument
                {
                    PredecessorId = _documentId3,
                    SectionId = _documentId2,
                    DocumentId = _documentId1
                });
                
                db.Insert(new TableSectionDocument
                {
                    PredecessorId = 0,
                    SectionId = _documentId3,
                    DocumentId = _documentId1
                });
            }
        }
        
        [Test]
        public void TestGetSectionsDocumentsByDocumentIdHasCorrectOrder()
        {
            var orderedSectionsDocuments =
                Docomposer.Core.Api.SectionsDocuments.GetSectionsDocumentsByDocumentId(_documentId1);

            Assert.That(orderedSectionsDocuments[0].SectionId, Is.EqualTo(3));
            Assert.That(orderedSectionsDocuments[0].PredecessorId, Is.EqualTo(0));
            
            Assert.That(orderedSectionsDocuments[1].SectionId, Is.EqualTo(2));
            Assert.That(orderedSectionsDocuments[1].PredecessorId, Is.EqualTo(_documentId3));
            
            Assert.That(orderedSectionsDocuments[2].SectionId, Is.EqualTo(1));
            Assert.That(orderedSectionsDocuments[2].PredecessorId, Is.EqualTo(_documentId2));
        }

        [TearDown]
        public void TearDown()
        {
            _migrator.CleanUp();
        }
    }
}