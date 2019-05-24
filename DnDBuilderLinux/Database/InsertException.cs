using System;

namespace DnDBuilderLinux.Database
{
    public class InsertException: Exception
    {
        public InsertException(string message, Exception innerException): base(message, innerException) { }

        public InsertException(string message): base(message) { }
    }
}