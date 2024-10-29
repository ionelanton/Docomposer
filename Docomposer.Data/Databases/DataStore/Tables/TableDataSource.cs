using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = data_source._)]
    public class TableDataSource
    {
        [PrimaryKey, Identity]
        [Column(Name = data_source.id)]
        public int Id { get; set; }

        [Column(Name = data_source.name)]
        public string Name { get; set; }
        
        [Column(Name = data_source.project_id)]
        public int ProjectId { get; set; }
        
        [Column(Name = data_source.type)]
        public DataSourceEnum Type { get; set; }

        [Column(Name = data_source.configuration)]
        public string Configuration { get; set; }
    }
}