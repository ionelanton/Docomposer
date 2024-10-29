using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = setting._)]
    public class TableSetting
    {
        [PrimaryKey]
        [Column(Name = setting.key, CanBeNull = false)]
        public string Key { get; set; }
        
        [Column(Name = setting.value)]
        public string Value { get; set; }
    }
}
