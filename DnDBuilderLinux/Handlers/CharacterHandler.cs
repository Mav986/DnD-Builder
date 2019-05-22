using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
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
        /// <param name="character">Character to add</param>
        /// <exception cref="CharacterException"></exception>
        public void AddCharacter(Character character)
        {
            try
            {
                _dndHandler.ValidateAbilityScores(character);
                _db.InsertCharacter(character);
            }
            catch (DatabaseException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }

        /// <summary>
        ///     Get all characters currently stored in DnDBuilder
        /// </summary>
        /// <returns>A JArray containing all character's details</returns>
        /// <exception cref="CharacterException"></exception>
        public JArray GetAllCharacters()
        {
            try
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
            catch (DatabaseException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }

        /// <summary>
        ///     Get a single character from within DnDBuilder
        /// </summary>
        /// <param name="name">Name of the character to retreive</param>
        /// <returns>A JObject containing the specified character's details</returns>
        /// <exception cref="CharacterException"></exception>
        public JObject GetCharacter(string name)
        {
            try
            {
                Character selectedChar = _db.SelectCharacter(name);
                JObject json = JObject.FromObject(selectedChar);

                return AddCalculatedAttributes(json);
            }
            catch (DatabaseException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }

        /// <summary>
        ///     Update a single character within DnDBuilder
        /// </summary>
        /// <param name="json">A JObject containing a character's name and the details to be updated</param>
        /// <exception cref="CharacterException"></exception>
        public void UpdateCharacter(JObject json)
        {
            try
            {
                string name = (string) json[Schema.Character.Field.Name];
                json.Remove(Schema.Character.Field.Name);
                
                Dictionary<string, string> propertyDict = GeneratePropertyDict(json);
                _db.UpdateCharacter(name, propertyDict);
            }
            catch (DatabaseException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }

        /// <summary>
        ///     Delete a single character from DnDBuilder
        /// </summary>
        /// <param name="name">Name of character to delete</param>
        /// <exception cref="CharacterException"></exception>
        public void DeleteCharacter(string name)
        {
            try
            {
                _db.DeleteCharacter(name);
            }
            catch (DatabaseException e)
            {
                throw new CharacterException(e.Message, e);
            }
        }

        /// <summary>
        ///     Generate an XML file representing a single character
        /// </summary>
        /// <param name="name">Name of character to generate XML file for</param>
        /// <param name="filename">Name of the file to generate</param>
        public void CreateCharacterXml(string name, string filename)
        {
            JObject charJson = GetCharacter(name);
            Character character = charJson.ToObject<Character>();
            XmlSerializer writer = new XmlSerializer(typeof(Character));
            FileStream file = File.Create(filename);

            writer.Serialize(file, character);
            file.Close();
        }

        /// <summary>
        ///     Add calculated attributes to character details JSON
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private JObject AddCalculatedAttributes(JObject json)
        {
            json["hitpoints"] = _dndHandler.CalculateHitpoints(json);
            json["caster"] = _dndHandler.CalculateCaster(json);

            return json;
        }

        /// <summary>
        ///     Generate a dictionary containing property : value mappings from a JObject
        /// </summary>
        /// <param name="json">JObject to generate mappings from</param>
        /// <returns>A dictionary containing all property : value mappings from the JObject</returns>
        /// <exception cref="CharacterException"></exception>
        private Dictionary<string, string> GeneratePropertyDict(JObject json)
        {
            Dictionary<string, string> characterDict = new Dictionary<string, string>();
            
            foreach (JProperty property in json.Properties())
            {
                string key = property.Name;
                string value = property.Value.ToString();
                
                characterDict.Add(key, value);
            }

            if (characterDict.Count < 1) throw new CharacterException("No fields updated");

            return characterDict;
        }
    }
}