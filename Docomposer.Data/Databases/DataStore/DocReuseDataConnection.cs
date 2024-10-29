using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Util;
using LinqToDB;
using Microsoft.Extensions.Configuration;

namespace Docomposer.Data.Databases.DataStore
{
    public sealed class DocReuseDataConnection : LinqToDB.Data.DataConnection
    {
        public DocReuseDataConnection() : base(
            providerName: ProviderName.SqlCe,
            connectionString: DatabaseConnections.DataStoreConnectionString())
        {
        }

        public DocReuseDataConnection(IConfiguration configuration) : base(
            ProviderName.SqlCe, DatabaseConnections.DataStoreConnectionString())
        {
        }
        
        public DocReuseDataConnection(string providerName, string connectionString) : base(
            providerName, connectionString)
        {
        }
        
        public ITable<TableVersionInfo> VersionInfo => this.GetTable<TableVersionInfo>();
        public ITable<TableUser> User => this.GetTable<TableUser>();
        public ITable<TableRole> Role => this.GetTable<TableRole>();
        public ITable<TableUserRole> UserRole => this.GetTable<TableUserRole>();
        public ITable<TableProject> Project => this.GetTable<TableProject>();
        public ITable<TableDataSource> DataSource => this.GetTable<TableDataSource>();
        public ITable<TableSection> Section => this.GetTable<TableSection>();
        public ITable<TableDocument> Document => this.GetTable<TableDocument>();
        public ITable<TableSectionDocument> SectionDocument => this.GetTable<TableSectionDocument>();
        public ITable<TableComposition> Composition => this.GetTable<TableComposition>();
        public ITable<TableDocumentComposition> DocumentComposition => this.GetTable<TableDocumentComposition>();
        public ITable<TableDataQuery> DataQuery => this.GetTable<TableDataQuery>();
        public ITable<TableWorkflow> Workflow => this.GetTable<TableWorkflow>();
        public ITable<TableSetting> Setting => this.GetTable<TableSetting>();
    }
}
