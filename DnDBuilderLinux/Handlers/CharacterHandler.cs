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
        
        /// <summary>
        ///     Add a character to DnDBuilder
        /// </summary>
        /// <param name="charData"></param>
        public void AddCharacter(Character charData)
        {
            _dndHandler.ValidateAbilityScores(charData);
            _db.InsertCharacter(charData);
        }

        public JArray GetAllCharacters()
        {
            IEnumerable<Character> charList = _db.SelectAllCharacters();
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

        public JObject GetCharacter(string name)
        {
            Character selectedChar = _db.SelectCharacter(name);
            JObject json = JObject.FromObject(selectedChar);

            return AddCalculatedAttributes(json);
        }

        public void UpdateCharacter(JObject charData)
        {
            Dictionary<string, string> characterDict = new Dictionary<string, string>();
            
            string name = (string) charData[Schema.Character.Field.Name];
            charData.Remove(Schema.Character.Field.Name);
            
            foreach (JProperty property in charData.Properties())
            {
                string key = property.Name;
                string value = property.Value.ToString();
                
                characterDict.Add(key, value);
            }
            _db.UpdateCharacter(name, characterDict);
        }

        private JObject AddCalculatedAttributes(JObject json)
        {
            json["hitpoints"] = _dndHandler.CalculateHitpoints(json);
            json["caster"] = _dndHandler.CalculateCaster(json);

            return json;
        }

        public void DeleteCharacter(string name)
        {
            _db.DeleteCharacter(name);
        }
    }
}