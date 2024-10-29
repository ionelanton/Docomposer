using System;
using System.IO;
using Docomposer.Core.Domain;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Test.Database;
using Docomposer.Data.Util;
using Docomposer.Utils;
using LinqToDB;
using NUnit.Framework;

namespace Docomposer.Core.Test.Api
{
    [TestFixture]
    public class TestDocuments : TestDatabase
    {
        private int _projectId, _documentId;
        private const string DocumentName = "A Document";

        [SetUp]
        public void SetUp()
        {
            using (var db = new DocReuseDataConnection())
            {
                _projectId = db.InsertWithInt32Identity(new TableProject
                {
                    Name = "Project"
                });

                _documentId = Docomposer.Core.Api.Documents.AddDocument(new Document
                {
                    Name = DocumentName,
                    ProjectId = _projectId
                });
            }
        }
        
        [Test]
        public void TestThrowsWhenDeletingDocumentHavingSectins()
        {
            using (var db = new DocReuseDataConnection())
            {
                var sectionId = db.InsertWithInt32Identity(new TableSection
                {
                    Name = "Section",
                    ProjectId = _projectId,
                });

                db.InsertWithInt32Identity(new TableSectionDocument
                {
                    PredecessorId = 0,
                    SectionId = sectionId,
                    DocumentId = _documentId
                });
            }

            Assert.Throws<ArgumentException>(() =>
            {
                Docomposer.Core.Api.Documents.DeleteDocument(_documentId);
            });
        }

        
        [Test]
        public void TestDeletingDocumentDeletesCompiledDocumentsAlso()
        {
            var pdf = Docomposer.Core.Api.Documents.GetDocumentAsPdf(_documentId);
            
            var cachedPdf = Path.Combine(ThisApp.DocReuseCacheDirectory(), _projectId.ToString(), DirName.CompiledDocuments, _documentId + ".pdf");
            var cachedDocx = Path.Combine(ThisApp.DocReuseCacheDirectory(), _projectId.ToString(), DirName.CompiledDocuments, DocumentName + ".docx");

            Assert.True(File.Exists(cachedPdf));
            Assert.True(File.Exists(cachedDocx));
            
            Docomposer.Core.Api.Documents.DeleteDocument(_documentId);
            
            Assert.False(File.Exists(cachedPdf));
            Assert.False(File.Exists(cachedDocx));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            using (var db = new DocReuseDataConnection())
            {
                db.Document.Delete(d => d.Id == _documentId);
                db.Project.Delete(c => c.Id == _projectId);
            }
        }
    }
}