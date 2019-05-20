using System;

namespace DnDBuilderLinux.Database
{
    // TODO Refactor into a couple different exceptions to be more verbose when throwing
    public class DatabaseException: Exception
    {
        public DatabaseException(string message, Exception innerException): base(message, innerException) { }

        public DatabaseException(string message): base(message) { }
    }
}