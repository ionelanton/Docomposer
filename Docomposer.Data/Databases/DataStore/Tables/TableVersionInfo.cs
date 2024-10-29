using System;
using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = version_info._)]
    public class TableVersionInfo
    {
        [PrimaryKey]
        [Column(Name = version_info.version)]
        public int Version { get; set; }

        [Column(Name = version_info.applied_on)]
        public DateTime AppliedOn { get; set; }

        [Column(Name = version_info.description)]
        public string Description { get; set; }
    }
}
