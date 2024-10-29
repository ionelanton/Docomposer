using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = workflow._)]
    public class TableWorkflow
    {
        [PrimaryKey, Identity]
        [Column(Name = workflow.id)]
        public int Id { get; set; }

        [Column(Name = workflow.project_id)]
        public int ProjectId { get; set; }

        [Column(Name = workflow.name)]
        public string Name { get; set; }
        
        [Column(Name = workflow.configuration)]
        public string Configuration { get; set; }
    }
}
