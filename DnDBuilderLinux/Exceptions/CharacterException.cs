using System;

namespace DnDBuilderLinux.Exceptions
{
    public class CharacterException: Exception
    {
        public CharacterException(string message, Exception innerException) : base(message, innerException) { }

        public CharacterException(string message) : base(message) { }
    }
}