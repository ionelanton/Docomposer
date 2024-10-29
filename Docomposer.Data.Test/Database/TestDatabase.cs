using System;
using System.Linq;
using System.Threading;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.Util;
using NUnit.Framework;

namespace Docomposer.Data.Test.Database
{
    [TestFixture]
    public class TestDatabase
    {
        [OneTimeSetUp]
        public void SetupDatabase()
        {
            using (var db = new DocReuseDataConnection())
            {
                try
                {
                    var versionInfo = db.VersionInfo.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    AppStoresInitializer.Initialize(true);
                    WaitForDatabaseInitialization();
                }
            }
        }

        private static void WaitForDatabaseInitialization()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }
}