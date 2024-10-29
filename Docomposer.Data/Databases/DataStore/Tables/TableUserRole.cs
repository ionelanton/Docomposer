using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = user_role._)]
    public class TableUserRole
    {
        [Column(Name = user_role.user_id)]
        public int UserId { get; set; }

        [Column(Name = user_role.role_id)]
        public int RoleId { get; set; }
    }
}
