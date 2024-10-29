using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = project._)]
    public class TableProject
    {
        [PrimaryKey, Identity]
        [Column(Name = project.id)]
        public int Id { get; set; }
        
        [Column(Name = project.parent_id, CanBeNull = true)]
        public int? ParentId { get; set; }

        [Column(Name = project.name)]
        public string Name { get; set; }
    }
}
