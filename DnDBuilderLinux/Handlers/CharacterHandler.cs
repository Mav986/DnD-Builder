using System;
using DnDBuilderLinux.Models;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Handlers
{
    public class CharacterHandler
    {
        public void AddCharacter(JObject charData)
        {
            try
            {
                Character newChar = charData.ToObject<Character>();
            }
            catch (ArgumentException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }
    }
}