using System;
using System.IO;
using Docomposer.Utils;

namespace Docomposer.Data.Util
{
    public static class DatabaseConnections
    {
        public static string DataStoreConnectionString()
        {
            var password = string.IsNullOrEmpty(ThisApp.AppSecurity.DataStorePassword)
                ? string.Empty
                : $"password={ThisApp.AppSecurity.DataStorePassword}";

            return $"Data Source={Path.Combine(ThisApp.AppDataDirectory(), ThisApp.DataStore())};{password}";
        }
    }
}