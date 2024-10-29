using System;
using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = role._)]
    public class TableRole
    {
        [PrimaryKey, Identity]
        [Column(Name = role.id)]
        public int Id { get; set; }

        [Column(Name = role.name), NotNull]
        public string Name { get; set; }

        [Column(Name = role.concurrency_stamp)]
        public DateTime? ConcurrencyStamp { get; set; }
    }
}
