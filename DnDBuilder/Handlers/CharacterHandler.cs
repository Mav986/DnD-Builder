using System;
using DnDBuilder.Web;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Handlers
{
    public class CharacterHandler
    {
        private readonly RequestHandler _reqHandler;

        /// <summary>
        ///     Retrieves race data from dnd5eapi.co
        /// </summary>
        /// <param name="reqHandler">A RequestHandler object</param>
        public CharacterHandler(RequestHandler reqHandler)
        {
            _reqHandler = reqHandler;
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
        ///     Get a specific race by name
        /// </summary>
        /// <param name="name">A valid 5e race name</param>
        /// <returns>JObject containing details of the requested race</returns>
        /// <exception cref="ArgumentException"></exception>
        public JObject GetRace(string name)
        {
            var allRaces = GetRaces()["results"];
            var json = _reqHandler.ExtractFromJArray(allRaces, name);

            return json;
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
        ///     Get a specific class by name
        /// </summary>
        /// <param name="name">A valid 5e class name</param>
        /// <returns>JObject containing details of the requested class</returns>
        /// <exception cref="ArgumentException"></exception>
        public JObject GetClass(string name)
        {
            var allClasses = GetClasses()["results"];
            var json = _reqHandler.ExtractFromJArray(allClasses, name);

            return json;
        }
    }
}