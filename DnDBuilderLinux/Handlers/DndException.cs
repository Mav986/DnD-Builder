using System;

namespace DnDBuilderLinux.Handlers
{
    public class DndException : Exception
    {
        public DndException(string message) : base(message) { }
    }
}