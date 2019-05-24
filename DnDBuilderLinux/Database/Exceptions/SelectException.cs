using System;

namespace DnDBuilderLinux.Database.Exceptions
{
    public class SelectException: Exception
    {
        public SelectException(string message, Exception innerException): base(message, innerException) { }

        public SelectException(string message): base(message) { }
    }
}