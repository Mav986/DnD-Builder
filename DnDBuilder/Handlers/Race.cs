using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Handlers
{
    public class Race
    {
        private readonly RequestHandler _client;
        private readonly JObjectCache _cache;
        
        /// <summary>
        /// Retrieves DnD 5e data from the supplied url 
        /// </summary>
        /// <param name="baseUri">A string containing the base url</param>
        public Race(string baseUri)
        {
            _client = new RequestHandler(baseUri);
            _cache = new JObjectCache();
        }

        /// <summary>
        /// Retrieves racial data from the base url
        /// </summary>
        /// <returns>JSON object containing the official 5e races</returns>
        public JObject GetRaces()
        {
            const string racesKey = "allRaces";
            const string racesUrl = "races";
            
            return GetJObject(racesKey, racesUrl);
        }

        /// <summary>
        /// Get details for a single race by name
        /// </summary>
        /// <param name="name">The name of the race to get details for</param>
        /// <returns>JObject containing details of the requested race</returns>
        /// <exception cref="ArgumentException"></exception>
        public JObject GetRace(string name)
        {
            JArray allRaces = (JArray)GetRaces()["results"];
            JObject json = allRaces.Children<JObject>()
                .FirstOrDefault(r => r["name"].ToString().ToLower().Equals(name));

            if (json == null) throw new ArgumentException($"{name} not found");

            string raceKey = json["name"].ToString();
            string raceUrl = json["url"].ToString();
            
            return GetJObject(raceKey, raceUrl);
        }

        /// <summary>
        /// Get a JObject from the cache or remote server
        /// </summary>
        /// <param name="key">A unique key to locate a cache item</param>
        /// <param name="url">A url to call if item is not in the cache</param>
        /// <returns>The requested JObject</returns>
        private JObject GetJObject(string key, string url)
        {
            JObject res;
            
            if (_cache.Contains(key))
            {
                res = _cache.Get(key);
            }
            else
            {
                res = _client.GetJson(url);
                _cache.Add(key, res);
            }

            return res;
        }
    }
}