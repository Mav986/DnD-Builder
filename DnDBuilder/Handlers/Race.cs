using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Handlers
{
    public class Race
    {
        // Endpoints
        private const string Races = "races";
        
        private readonly HttpClient _client;
        
        public Race(HttpClient client)
        {
            _client = client;

        }

        public JObject GetRaces()
        {
            HttpResponseMessage res = _client.GetAsync(Races).Result;
            res.EnsureSuccessStatusCode();
            JObject races = res.Content.ReadAsAsync<JObject>().Result;
            
            return races;
        }
    }
}