using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using DnDBuilderLinux.Handlers.Exceptions;
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
        ///     CachedGet all race names
        /// </summary>
        /// <returns>JSON array containing the official 5e race names</returns>
        public JArray GetAllRaces()
        {
            try
            {
                return GetNameArray(GetRacesFromCache);
            }
            catch (HttpRequestException e)
            {
                throw new DndException("Error retreiving races.", e);
            }
        }

        /// <summary>
        ///     CachedGet all class names
        /// </summary>
        /// <returns>JSON array containing the official 5e race names</returns>
        /// <exception cref="DndException"></exception>
        public JArray GetAllClasses()
        {
            try
            {
                return GetNameArray(GetClassesFromCache);
            }
            catch (HttpRequestException e)
            {
                throw new DndException("Error retreiving classes.", e);
            }
        }

        /// <summary>
        ///     Determine whether this character is a caster or not
        /// </summary>
        /// <param name="character">A valid dnd Character object</param>
        /// <returns>True if this character is a caster, false otherwise</returns>
        public bool CalculateCaster(JObject character)
        {
            JToken classType = character["class"] ?? "";
            return IsCaster(classType.ToString());
        }

        public bool IsCaster(string classType)
        {
            JObject classJson = GetClassJson(classType);
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
            try
            {
                JToken classType = character["class"];
                
                if (classType == null) throw new DndException("Class type cannot be null.");
                
                JObject classData = GetClassJson(classType.ToString());
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
            catch (InvalidCastException e)
            {
                throw new DndException("Unable to determine hitpoints for specified character", e);
            }
        }

        /// <summary>
        ///     CachedGet a JArray of names from a JObject retreived from the callback
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        /// <exception cref="DndException"></exception>
        private static JArray GetNameArray(Func<JObject> callback)
        {
            JObject json = callback();
            JArray nameList = ExtractNamesToArray(json);

            return nameList;
        }

        /// <summary>
        ///     CachedGet a JObject of races from the DnDBuilder cache
        /// </summary>
        /// <returns>A JObject containing a JArray of race name to url mappings</returns>
        private JObject GetRacesFromCache()
        {
            const string allRaces = "allRaces";
            const string url = "races";
            return _reqHandler.GetFromCache(allRaces, url);
        }

        /// <summary>
        ///     CachedGet a JObject of classes from the DnDBuilder cache
        /// </summary>
        /// <returns>A JObject containing a JArray of class name to url mappings</returns>
        private JObject GetClassesFromCache()
        {
            const string allClasses = "allClasses";
            const string url = "classes";
            return _reqHandler.GetFromCache(allClasses, url);
        }

        /// <summary>
        ///     CachedGet a dnd5eapi url from a JToken by name
        /// </summary>
        /// <param name="array">A JToken containing a list of name and url mappings</param>
        /// <param name="name">A valid dnd5eapi name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string GetUrlFromJToken(JToken array, string name)
        {
            JObject nameAndUrl = array.Children<JObject>()
                .FirstOrDefault(r => r["name"].ToString().Equals(name, StringComparison.OrdinalIgnoreCase));

            if (nameAndUrl == null) throw new ArgumentException($"{name} not found");

            return nameAndUrl["url"].ToString();
        }

        /// <summary>
        ///     CachedGet a JObject containing a classes data
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private JObject GetClassJson(string name)
        {
            JObject json = GetClassesFromCache();
            string url = GetUrlFromJToken(json["results"], HttpUtility.HtmlEncode(name));

            return _reqHandler.GetFromCache(name, url);
        }

        /// <summary>
        ///     Extract names from a JObject generated by dnd5eapi.co
        /// </summary>
        /// <param name="json">JObject to extract names from</param>
        /// <returns>A JArray containing strings</returns>
        private static JArray ExtractNamesToArray(JObject json)
        {
            JArray names = new JArray();
            
            foreach (JToken entry in json["results"])
            {
                names.Add(entry["name"].ToString());
            }

            return names;
        }
    }
}