using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{

    [Table(Name = section._)]
    public class TableSection : ITable
    {
        [PrimaryKey, Identity]
        [Column(Name = section.id)]
        public int Id { get; set; }

        [Column(Name = section.project_id)]
        public int ProjectId { get; set; }

        [Column(Name = section.name)]
        public string Name { get; set; }

        [Column(Name = section.editing_by)]
        public string EditingBy { get; set; }
    }
}
