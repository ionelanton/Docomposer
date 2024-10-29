using System.Collections.Generic;
using System.Linq;
using Docomposer.Data.Databases.Util;
using LinqToDB.Configuration;

namespace Docomposer.Data.Databases.DataStore.Util
{
    public class DatabaseSettings : ILinqToDBSettings
    {
        private readonly string _name;
        private readonly string _connectionString;

        public string DefaultConfiguration { get; }
        public string DefaultDataProvider { get; }

        public DatabaseSettings(string name, string providerName, string connectionString)
        {
            _name = name;
            _connectionString = connectionString;
            DefaultConfiguration = providerName;
            DefaultDataProvider = providerName;
        }

        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();
        
        public IEnumerable<IConnectionStringSettings> ConnectionStrings { get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = _name,
                        ProviderName = DefaultDataProvider,
                        ConnectionString = _connectionString
                    };
            }
        }
    }
}
