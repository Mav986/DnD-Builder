using System;

namespace DnDBuilderLinux.Exceptions
{
    public class DuplicateKeyException: Exception
    {
        public DuplicateKeyException(string message, Exception innerException): base(message, innerException) { }

        public DuplicateKeyException(string message): base(message) { }
    }
}