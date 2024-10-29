using System;

namespace Docomposer.Data.Util
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base("Database: " + message)
        {
        }
    }
}