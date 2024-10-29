using Docomposer.Core.Domain.DataSourceConfig;
using Docomposer.Data.Databases.DataStore.Tables;

namespace Docomposer.Blazor.Data.DataSource
{
    public class ItemDataSourceModel
    {
        public int DataSourceId { get; set; }
        public DataSourceEnum Type { get; set; }
        public IDataSourceConfig Config { get; set; }
        public int ProjectId { get; set; }
    }
}