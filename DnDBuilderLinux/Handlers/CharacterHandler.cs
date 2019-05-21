using System;
using DnDBuilderLinux.Database;
using DnDBuilderLinux.Models;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Handlers
{
    public class CharacterHandler
    {
        private readonly DatabaseHandler _db;
        
        public CharacterHandler(DatabaseHandler db)
        {
            _db = db;
        }
        
        public void AddCharacter(JObject charData)
        {
            try
            {
                if (charData == null) throw new ArgumentException("No character data specified");
                Character newChar = charData.ToObject<Character>();
                _db.AddCharacter(newChar);

            }
            catch (ArgumentException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }
    }
}