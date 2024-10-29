using LinqToDB.Configuration;

namespace Docomposer.Data.Databases.DataStore.Util
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal { get; set; }
    }
}
