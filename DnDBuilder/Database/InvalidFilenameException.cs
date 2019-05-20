using System;

namespace DnDBuilder.Database
{
    public class InvalidFilenameException: Exception
    {
        public InvalidFilenameException(string message, Exception innerException): base(message, innerException) { }
    }
}