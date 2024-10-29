using System;
using System.Data.SqlServerCe;
using System.IO;
using Docomposer.Data.Util;
using Docomposer.Utils;

namespace Docomposer.Data.Databases.DataStore
{
    public static class DocReuseDatabaseCreator
    {
        public static void Create(bool force = false)
        {
            var dbDataFile = Path.Combine(ThisApp.AppDataDirectory(), ThisApp.DataStore());
            if (File.Exists(dbDataFile))
            {
                if (force)
                {
                    File.Delete(dbDataFile);
                }
                else
                {
                    throw new ArgumentException($"File {dbDataFile} exists");
                }
            }
            
            using var engine = new SqlCeEngine(DatabaseConnections.DataStoreConnectionString());
            engine.CreateDatabase();
        }
    }
}