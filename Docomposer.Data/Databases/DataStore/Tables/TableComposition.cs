using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = composition._)]
    public class TableComposition
    {
        [PrimaryKey, Identity]
        [Column(Name = composition.id)]
        public int Id { get; set; }

        [Column(Name = composition.project_id)]
        public int ProjectId { get; set; }

        [Column(Name = composition.name)]
        public string Name { get; set; }
    }
}
