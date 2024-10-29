using System;
using System.Data;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Util;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Docomposer.Data.Test.Database
{
    [TestFixture]
    internal class TestDocReuseDataConnection : TestDatabase
    {
        private DatabaseMigrator _migrator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _migrator = new DatabaseMigrator();
            _migrator.CleanUp();
        }

        [Test]
        public void TestDatabaseConnection()
        {
            using var db = new DocReuseDataConnection();

            Assert.That(db.ConnectionString, Contains.Substring("app.data"));
            Assert.That(db.Connection.State, Is.EqualTo(ConnectionState.Open));
        }

        [Test]
        public void TestM201911122107CreateDatabase()
        {
            _migrator.Execute();
            using var db = new DocReuseDataConnection();

            Assert.That(db.User, Is.Empty);
            Assert.That(db.Role, Is.Empty);
            Assert.That(db.UserRole, Is.Empty);
            Assert.That(db.Project, Is.Empty);
            Assert.That(db.DataSource, Is.Empty);
            Assert.That(db.Section, Is.Empty);
            Assert.That(db.Document, Is.Empty);
            Assert.That(db.SectionDocument, Is.Empty);
            Assert.That(db.Composition, Is.Empty);
            Assert.That(db.DocumentComposition, Is.Empty);
            Assert.That(db.DataQuery, Is.Empty);
            Assert.That(db.Workflow, Is.Empty);
        }

        [Test]
        public void TestExcelDataSource()
        {
            
        }

        [Test]
        public void TestSqliteConnectionString()
        {
            var connectionString = new SqliteConnectionStringBuilder("Data Source=:memory:;")
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false,
                Cache = SqliteCacheMode.Default,
                Password = "a password"
            }.ToString();
            Console.Out.WriteLine("connectionString = {0}", connectionString);
        }
        

        [OneTimeTearDown]
        public void TearDown()
        {
            _migrator.CleanUp();
        }
    }
}