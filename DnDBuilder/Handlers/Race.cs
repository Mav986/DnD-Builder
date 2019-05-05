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
            const string cacheKey = "racesKey";
            const int expiryInMinutes = 10;
            JObject races;
            
            if (_cache.Contains(cacheKey))
            {
                races = _cache.Get(cacheKey);
            }
            else
            {
                races = _client.GetJson("races");
                _cache.Add(cacheKey, races, expiryInMinutes);
            }
            
            return races;
        }
    }
}