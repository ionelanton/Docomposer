using System;
using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using LinqToDB.Mapping;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    [Table(Name = user._)]
    public class TableUser
    {
        [PrimaryKey, Identity]
        [Column(Name = user.id), NotNull]
        public int Id { get; set; }

        [Column(Name = user.user_name), NotNull]
        public string UserName { get; set; }

        [Column(Name = user.concurrency_stamp)]
        public DateTime? ConcurrencyStamp { get; set; }

        [Column(Name = user.first_name)]
        public string FirstName { get; set; }

        [Column(Name = user.last_name)]
        public string LastName { get; set; }

        [Column(Name = user.email), NotNull]
        public string Email { get; set; }

        [Column(Name = user.email_confirmed)]
        public bool EmailConfirmed { get; set; }

        [Column(Name = user.access_failed_count)]
        public int AccessFailedCount { get; set; }

        [Column(Name = user.lockout_enabled)]
        public bool LockoutEnabled { get; set; }

        [Column(Name = user.lockout_end)]
        public DateTime? LockoutEnd { get; set; }

        [Column(Name = user.password_hash)]
        public string PasswordHash { get; set; }

        [Column(Name = user.password_salt)]
        public string PasswordSalt { get; set; }
        
        [Column(Name = user.token)]
        public string Token { get; set; }
        
        [Column(Name = user.token_creation_time)]
        public DateTime TokenCreationTime { get; set; }
    }
}
