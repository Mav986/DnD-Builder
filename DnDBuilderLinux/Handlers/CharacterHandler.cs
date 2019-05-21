using System;
using DnDBuilderLinux.Database;
using DnDBuilderLinux.Models;

namespace DnDBuilderLinux.Handlers
{
    public class CharacterHandler
    {
        private readonly DatabaseHandler _db;
        private readonly Dnd5EHandler _dndHandler;
        
        public CharacterHandler(DatabaseHandler db)
        {
            _db = db;
            _dndHandler = new Dnd5EHandler();
        }
        
        public void AddCharacter(Character charData)
        {
            try
            {
                charData.Caster = _dndHandler.IsCaster(charData.Class);
                _db.AddCharacter(charData);
            }
            catch (ArgumentException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }
    }
}