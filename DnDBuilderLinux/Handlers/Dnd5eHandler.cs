using System;
using System.Linq;
using DnDBuilderLinux.Models;
using DnDBuilderLinux.Web;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Handlers
{
    public class Dnd5EHandler
    {
        private readonly RequestHandler _reqHandler;

        /// <summary>
        ///     Retrieves race data from dnd5eapi.co
        /// </summary>
        public Dnd5EHandler()
        {
            _reqHandler = new RequestHandler("http://www.dnd5eapi.co/api/", new CacheHandler());
        }

        /// <summary>
        ///     Get all races
        /// </summary>
        /// <returns>JSON object containing the official 5e races</returns>
        public JObject GetRaces()
        {
            const string key = "allRaces";
            const string url = "races";

            return _reqHandler.GetFromCache(key, url);
        }

        /// <summary>
        ///     Get a dnd5eapi url from JToken by name
        /// </summary>
        /// <param name="array">A JToken containing a list of name and url mappings</param>
        /// <param name="name">A valid dnd5eapi name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GetUrlFromJToken(JToken array, string name)
        {
            JObject nameAndUrl = array.Children<JObject>()
                .FirstOrDefault(r => r["name"].ToString().Equals(name, StringComparison.OrdinalIgnoreCase));

            if (nameAndUrl == null) throw new ArgumentException($"{name} not found");

            return nameAndUrl["url"].ToString();;
        }

        /// <summary>
        ///     Get all classes
        /// </summary>
        /// <returns>JSON object containing the official 5e classes</returns>
        public JObject GetClasses()
        {
            const string key = "allClasses";
            const string url = "classes";

            return _reqHandler.GetFromCache(key, url);
        }

        /// <summary>
        ///     Determine whether this character is a caster or not
        /// </summary>
        /// <param name="character">A valid dnd Character object</param>
        /// <returns>True if this character is a caster, false otherwise</returns>
        public bool CalculateCaster(JObject character)
        {
            JObject classJson = GetClassJson(character["class"].ToString());
            return classJson["spellcasting"] != null;
        }

        /// <summary>
        ///     Determine a character's hit die
        /// </summary>
        /// <param name="character">A valid dnd Character object</param>
        /// <returns>The character's hit die</returns>
        /// <exception cref="DndException">If a character's hit die cannot be determined</exception>
        public long CalculateHitpoints(JObject character)
        {
            JObject classData = GetClassJson(character["class"].ToString());
            bool hitDieFound = classData.TryGetValue("hit_die", out JToken hitDieToken);
            bool levelFound = character.TryGetValue("level", out JToken levelToken);
            bool conFound = character.TryGetValue("con", out JToken conToken);

            if (!hitDieFound || !levelFound || !conFound)
                throw new DndException("Unable to determine hitpoints for " + character["name"]);
            
            long hitDie = (long) hitDieToken;
            long level = (long) levelToken;
            long con = (long) conToken;

            return level * hitDie + con;
        }

        /// <summary>
        ///     Validate a character's total ability scores.
        ///     A character's ability scores are considered valid if they add up to a total of 20.
        /// </summary>
        /// <param name="character"></param>
        /// <exception cref="DndException"></exception>
        public void ValidateAbilityScores(Character character)
        {
            const int totalAbilityScore = 20;
            long abilityScore = character.Con + character.Dex + character.Str + 
                                character.Cha + character.Intel + character.Wis;
            
            if(abilityScore != totalAbilityScore) throw new DndException("Total ability score '" + abilityScore +"' invalid");
        }

        /// <summary>
        ///     Get a JObject containing a classes data
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private JObject GetClassJson(string name)
        {
            JObject json = GetClasses();
            string url = GetUrlFromJToken(json["results"], name);

            return _reqHandler.GetFromCache(name, url);
        }
    }
}