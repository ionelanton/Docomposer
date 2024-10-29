namespace Docomposer.Data.Databases.DataStore.Tables.Definitions
{
    public class user
    {
        public const string _ = "user";
        // columns
        public const string id = "id";
        public const string user_name = "user_name";
        public const string concurrency_stamp = "concurrency_stamp";
        public const string first_name = "first_name";
        public const string last_name = "last_name";
        public const string email = "email";
        public const string email_confirmed = "email_confirmed";
        public const string access_failed_count = "access_failed_count";
        public const string lockout_enabled = "lockout_enabled";
        public const string lockout_end = "lockout_end";
        public const string password_hash = "password_hash";
        public const string password_salt = "password_salt";
        public const string token = "access_token";
        public const string token_creation_time = "token_creation_time";
    }
}
