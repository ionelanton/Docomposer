using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = document._)]
    public class TableDocument
    {
        [PrimaryKey, Identity]
        [Column(Name = document.id)]
        public int Id { get; set; }

        [Column(Name = document.project_id)]
        public int ProjectId { get; set; }

        [Column(Name = document.name)]
        public string Name { get; set; }
    }
}
