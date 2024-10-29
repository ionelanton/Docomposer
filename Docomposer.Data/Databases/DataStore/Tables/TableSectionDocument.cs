using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = section_x_document._)]
    public class TableSectionDocument
    {
        [PrimaryKey, Identity]
        [Column(Name = section_x_document.id)]
        public int Id { get; set; }

        [Column(Name = section_x_document.section_id)]
        public int SectionId { get; set; }

        [Column(Name = section_x_document.document_id)]
        public int DocumentId { get; set; }

        [Column(Name = section_x_document.predecessor_id)]
        public int PredecessorId { get; set; }
    }
}
