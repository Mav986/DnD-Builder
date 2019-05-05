using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Handlers
{
    public class RequestHandler
    {
        private readonly HttpClient _client;
        
        /// <summary>
        /// Handler for making http requests
        /// </summary>
        /// <param name="url">A string containing the base url to make requests to</param>
        public RequestHandler(string url)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }
        
        /// <summary>
        /// Request a JObject from an endpoint
        /// </summary>
        /// <param name="endpoint">A string indicating the endpoint to request from</param>
        /// <returns>A JObject of the response body JSON</returns>
        public JObject GetJson(string endpoint)
        {
            HttpResponseMessage res = _client.GetAsync(endpoint).Result;
            res.EnsureSuccessStatusCode();
            JObject json = res.Content.ReadAsAsync<JObject>().Result;

            return json;
        }
    }
}