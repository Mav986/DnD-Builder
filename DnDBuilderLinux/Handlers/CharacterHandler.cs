using System;
using System.Collections.Generic;
using DnDBuilderLinux.Database;
using DnDBuilderLinux.Models;
using Newtonsoft.Json.Linq;

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

        public JArray GetAllCharacters()
        {
            List<Character> charList = _db.SelectAll();
            JArray jsonList = new JArray();

            foreach (Character c in charList)
            {
                JObject charDetails = new JObject
                {
                    [Schema.Character.Field.Name] = c.Name, 
                    [Schema.Character.Field.Race] = c.Race, 
                    [Schema.Character.Field.Class] = c.Class, 
                    [Schema.Character.Field.Level] = c.Level
                };
                
                jsonList.Add(charDetails);
            }

            return jsonList;
        }

        public void UpdateCharacter(JObject newData)
        {
            try
            {
                string name = (string) newData[Schema.Character.Field.Name];
                newData.Remove(Schema.Character.Field.Name);
                foreach (JProperty property in newData.Properties())
                {
                    string key = property.Name;
                    string value = property.Value.ToString();
                    _db.UpdateCharacter(name, key, value);
                }
            }   
            catch (DatabaseException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }
    }
}