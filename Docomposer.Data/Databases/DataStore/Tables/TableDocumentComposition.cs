using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = document_x_composition._)]
    public class TableDocumentComposition
    {
        [PrimaryKey, Identity]
        [Column(Name = document_x_composition.id)]
        public int Id { get; set; }

        [Column(Name = document_x_composition.composition_id)]
        public int CompositionId { get; set; }

        [Column(Name = document_x_composition.document_id)]
        public int DocumentId { get; set; }

        [Column(Name = document_x_composition.predecessor_id)]
        public int PredecessorId { get; set; }
    }
}
