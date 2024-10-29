using System.IO;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Util;
using Docomposer.Data.Util;
using Docomposer.Utils;

namespace Docomposer.Data.Databases.Util
{
    public static class AppStoresInitializer
    {
        public static void Initialize(bool force = false)
        {
            Directory.CreateDirectory(ThisApp.AppDataDirectory());
                
            // create data store
            DocReuseDatabaseCreator.Create(force);

            var dataStoreMigrator = new DatabaseMigrator();
            dataStoreMigrator.CleanUp();
            dataStoreMigrator.Execute();

            // populate data store
            new DatabasePopulator().Populate();
            
            // populate files
            new FilePopulator().Populate();
        }
    }
}