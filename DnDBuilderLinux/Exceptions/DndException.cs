using System;

namespace DnDBuilderLinux.Exceptions
{
    public class DndException : Exception
    {
        public DndException(string message) : base(message) { }
        public DndException(string message, Exception innerException) : base(message, innerException) { }
    }
}