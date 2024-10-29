using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = data_query._)]
    public class TableDataQuery
    {
        [PrimaryKey, Identity]
        [Column(Name = data_query.id)]
        public int Id { get; set; }
        
        [Column(Name = data_query.data_source_id)]
        public int DataSourceId { get; set; }
        
        [Column(Name = data_query.name)]
        public string Name { get; set; }

        [Column(Name = data_query.description)]
        public string Description { get; set; }

        [Column(Name = data_query.statement, Length = int.MaxValue)]
        public string Statement { get; set; }
        
        [Column(Name = data_query.parameters)]
        public string Parameters { get; set; }
    }
}