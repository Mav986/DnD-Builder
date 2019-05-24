using System;

namespace DnDBuilderLinux.Database.Exceptions
{
    public class InsertException: Exception
    {
        public InsertException(string message, Exception innerException): base(message, innerException) { }

        public InsertException(string message): base(message) { }
    }
}