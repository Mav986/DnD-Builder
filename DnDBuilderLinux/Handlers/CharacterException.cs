using System;

namespace DnDBuilderLinux.Handlers
{
    public class CharacterException: Exception
    {
        public CharacterException(string message, Exception innerException) : base(message, innerException) { }
    }
}