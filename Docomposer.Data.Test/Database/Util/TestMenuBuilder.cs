using System.Linq;
using Docomposer.Data.Databases;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Util;
using Docomposer.Data.Databases.Util;
using NUnit.Framework;

namespace Docomposer.Data.Test.Database.Util
{
    [TestFixture]
    internal class TestMenuBuilder : TestDatabase
    {
        private DatabaseMigrator _migrator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _migrator = new DatabaseMigrator();
            _migrator.CleanUp();
            _migrator.Execute();

            var populator = new DatabasePopulator();
            populator.Populate();
        }

        [Test]
        public void TestMenuHasElements()
        {
            using (var db = new DocReuseDataConnection())
            {
                var menus = db.Project.Select(c => c.Id > 0).ToList();

                Assert.That(menus, Has.Count.GreaterThan(0));
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _migrator.CleanUp();
        }
    }
}
