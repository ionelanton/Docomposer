using System;
using Docomposer.Core.Domain;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Test.Database;
using LinqToDB;
using NUnit.Framework;

namespace Docomposer.Core.Test.Api
{
    [TestFixture]
    public class TestCompositions : TestDatabase
    {
        private int _projectId, _compositionId, _documentId;
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

                _compositionId = db.InsertWithInt32Identity(new TableComposition
                {
                    Name = "Composition",
                    ProjectId = _projectId
                });
                
                _documentId = Docomposer.Core.Api.Documents.AddDocument(new Document
                {
                    Name = DocumentName,
                    ProjectId = _projectId
                });

                db.InsertWithInt32Identity(new TableDocumentComposition
                {
                    CompositionId = _compositionId,
                    PredecessorId = 0,
                    DocumentId = _documentId
                });
            }
        }
        
        [Test]
        public void TestThrowsWhenDeletingCompositionHavingDocuments()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Docomposer.Core.Api.Compositions.DeleteComposition(_compositionId);
            });
        }
    }
}