using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
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
        /// <param name="character">JSON object with required parameters to create a Character object</param>
        /// <exception cref="CharacterException"></exception>
        public void AddCharacter(JObject character)
        {
            try
            {
                Character newChar = CreateCharacter(character);
                _db.InsertCharacter(newChar);
            }
            catch (InsertException e)
            {
                throw new CharacterException(e.Message, e);
            }
            catch (DatabaseException e)
            {
                throw new CharacterException("Internal error while adding character.", e);
            }
        }

        /// <summary>
        ///     Get all characters currently stored in DnDBuilder
        /// </summary>
        /// <returns>A JArray containing JObjects, each of which has a small summary of each character</returns>
        /// <exception cref="CharacterException"></exception>
        public JArray GetAllCharacters()
        {
            try
            {
                DataTable characterTable = _db.SelectAllCharacters();
                JArray fullDetailsArray = JArray.FromObject(characterTable);
                JArray minDetailsArray = new JArray();

                foreach (JToken token in fullDetailsArray)
                {
                    JObject json = (JObject) token;
                    SanitizeJson(ref json);
                    ValidateJson(ref json);

                    JObject minified = CreateMinifiedJson(json);
                    minDetailsArray.Add(minified);
                }

                return minDetailsArray;
            }
            catch (DatabaseException e)
            {
                throw new CharacterException("Internal error while retrieving characters.", e);
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
                DataTable selectedChar = _db.SelectCharacter(name);
                JArray array = JArray.FromObject(selectedChar);
                JObject json = JObject.FromObject(array[0]);
                SanitizeJson(ref json);
                ValidateJson(ref json);

                return AddCalculatedAttributes(json);
            }
            catch (SelectException e)
            {
                throw new CharacterException(e.Message, e);
            }
            catch (DatabaseException e)
            {
                throw new CharacterException("Internal error while retrieving character.", e);
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
                throw new CharacterException("Error updating character in DnDBuilder.", e);
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
                throw new CharacterException("Error deleting character from DnDBuilder.", e);
            }
        }

        /// <summary>
        ///     Generate an XML file representing a single character
        /// </summary>
        /// <param name="name">Name of character to generate XML file for</param>
        /// <param name="filename">Name of the file to generate</param>
        public void CreateCharacterXml(string name, string filename)
        {
            try
            {
                JObject charJson = GetCharacter(name);
                Character character = charJson.ToObject<Character>();
                XmlSerializer writer = new XmlSerializer(typeof(Character));
                FileStream file = File.Create(filename);

                writer.Serialize(file, character);
                file.Close();
            }
            catch (IOException e)
            {
                throw new CharacterException("Error generating xml for character.", e);
            }
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
        private static Dictionary<string, string> GeneratePropertyDict(JObject json)
        {
            Dictionary<string, string> characterDict = new Dictionary<string, string>();
            
            foreach (JProperty property in json.Properties())
            {
                string key = property.Name;
                string value = property.Value == null ? "" : property.Value.ToString();
                
                characterDict.Add(key, value);
            }

            if (characterDict.Count < 1) throw new CharacterException("No fields to update.");

            return characterDict;
        }

        /// <summary>
        ///     Convert database reader results into a character object
        /// </summary>
        /// <param name="reader">A database reader holding results</param>
        /// <returns>A character object</returns>
        private static Character SanitizeCharacter(IDataRecord reader)
        {
            string name = reader[Schema.Character.Field.Name] as string;
            string gender = reader[Schema.Character.Field.Gender] as string;
            string bio = reader[Schema.Character.Field.Bio] as string;
            string race = reader[Schema.Character.Field.Race] as string;
            string classType = reader[Schema.Character.Field.Class] as string;

            return new Character
            {
                Name = string.IsNullOrEmpty(name) ? "" : HttpUtility.JavaScriptStringEncode(name),
                Gender = string.IsNullOrEmpty(gender) ? "" : HttpUtility.JavaScriptStringEncode(gender),
                Biography = string.IsNullOrEmpty(bio) ? "" : HttpUtility.JavaScriptStringEncode(bio),
                Race = string.IsNullOrEmpty(race) ? "" : HttpUtility.JavaScriptStringEncode(race),
                Class = string.IsNullOrEmpty(classType) ? "" : HttpUtility.JavaScriptStringEncode(classType),
                Age = reader[Schema.Character.Field.Age] as long? ?? 0,
                Level = reader[Schema.Character.Field.Level] as long? ?? 0,
                Con = reader[Schema.Character.Field.Constitution] as long? ?? 0,
                Dex = reader[Schema.Character.Field.Dexterity] as long? ?? 0,
                Str = reader[Schema.Character.Field.Strength] as long? ?? 0,
                Cha = reader[Schema.Character.Field.Charisma] as long? ?? 0,
                Intel = reader[Schema.Character.Field.Intelligence] as long? ?? 0,
                Wis = reader[Schema.Character.Field.Wisdom] as long? ?? 0
            };
        }

        /// <summary>
        ///     Create a Character object from JSON
        /// </summary>
        /// <param name="json">A JSON object containing character data</param>
        /// <returns>A Character object</returns>
        /// <exception cref="CharacterException">If the JSON data is invalid</exception>
        private Character CreateCharacter(JObject json)
        {
            try
            {
                SanitizeJson(ref json);
                CleanJson(ref json);
                ValidateJson(ref json);

                return json.ToObject<Character>();
            }
            catch (NullReferenceException e)
            {
                throw new CharacterException("Data invalid.", e);
            }
        }

        /// <summary>
        ///     Sanitize a JObject's values to prevent HTML XSS
        /// </summary>
        /// <param name="json">The JObject to sanitize</param>
        private void SanitizeJson(ref JObject json)
        {
            foreach (KeyValuePair<string, JToken> token in json)
            {
                json[token.Key] = HttpUtility.HtmlEncode(token.Value);
            }
        }

        /// <summary>
        ///     Ensure all fields in a JObject exist or are set to a default
        /// </summary>
        /// <param name="json">The JObject to clean</param>
        private void CleanJson(ref JObject json)
        {
            // 'variable ?? value' syntax is shorthand for "If variable is null, set it to value"
            json["name"] = json["name"] ?? "";
            json["gender"] = json["gender"] ?? "";
            json["bio"] = json["bio"] ?? "";
            json["race"] = json["race"] ?? "";
            json["class"] = json["class"] ?? "";
            json["age"] = json["age"] ?? 0;
            json["level"] = json["level"] ?? 1;
            json["con"] = json["con"] ?? 0;
            json["dex"] = json["dex"] ?? 0;
            json["str"] = json["str"] ?? 0;
            json["cha"] = json["cha"] ?? 0;
            json["intel"] = json["intel"] ?? 0;
            json["wis"] = json["wis"] ?? 0;
        }

        /// <summary>
        ///     Validate all character-related fields in a JObject
        /// </summary>
        /// <param name="json">The JObject to validate</param>
        /// <exception cref="CharacterException">If any of the fields are invalid</exception>
        private void ValidateJson(ref JObject json)
        {
            const int maxAbilityScores = 20;
            
            string name = json["name"].ToString();
            long age = (long) json["age"];
            string bio = json["bio"].ToString();
            long level = (long) json["level"];
            string race = json["race"].ToString();
            string classType = json["class"].ToString();
            long con = (long) json["con"];
            long dex = (long) json["dex"];
            long str = (long) json["str"];
            long cha = (long) json["cha"];
            long intel = (long) json["intel"];
            long wis = (long) json["wis"];
            long attributeTotal = con + dex + str + cha + intel + wis;
            
            JArray allRaces = _dndHandler.GetAllRaces();
            JArray allClasses = _dndHandler.GetAllClasses();
            
            bool racesValid = allRaces.Any(x => string.Equals(x.Value<string>(), race, StringComparison.OrdinalIgnoreCase));
            bool classesValid = allClasses.Any(x => string.Equals(x.Value<string>(), classType, StringComparison.OrdinalIgnoreCase));
            
            if (name.Length < 1) throw new CharacterException("Character name is required.");
            if (age < 0 || age > 500) throw new CharacterException("Age must be between 0 and 500.");
            if (bio.Length > 500) throw new CharacterException("Biography must not exceed 500 characters.");
            if (level < 1 || level > 20) throw new CharacterException("Level must be between 1 and 20.");
            if (!racesValid) throw new CharacterException("Invalid race.");
            if (!classesValid) throw new CharacterException("Invalid class.");
            if(attributeTotal != maxAbilityScores) 
                throw new CharacterException($"Invalid attribute scores, total points must equal {maxAbilityScores}.");
        }

        /// <summary>
        ///     Create a minified JSON containing only the name, race, class, and level of the character
        /// </summary>
        /// <param name="json">JObject to minify</param>
        /// <returns>A JObject with only the summarized attributes</returns>
        private static JObject CreateMinifiedJson(JObject json)
        {
            return new JObject
            {
                ["name"] = json["name"],
                ["race"] = json["race"],
                ["class"] = json["class"],
                ["level"] = json["level"]
            };
        }
    }
}