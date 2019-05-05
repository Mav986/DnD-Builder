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
        /// <param name="url">A string representing the url to get JSON from</param>
        /// <returns>A JObject of the response body JSON</returns>
        public JObject GetJson(string url)
        {
            string endpoint = ExtractEndpoint(url);
            HttpResponseMessage res = _client.GetAsync(endpoint).Result;
            res.EnsureSuccessStatusCode();
            JObject json = res.Content.ReadAsAsync<JObject>().Result;

            return json;
        }

        /// <summary>
        /// Extract an endpoint from the base url
        /// </summary>
        /// <param name="url">A base url of the form "domain.com/api/some-endpoint-route</param>
        /// <returns>The endpoint extracted from the base url</returns>
        private string ExtractEndpoint(string url)
        {
            string baseUrl = _client.BaseAddress.ToString();
            if (!url.StartsWith(baseUrl)) return url;
            string[] result = url.Split(new string[] {baseUrl}, StringSplitOptions.None);
            
            return string.Join("", result);
        }
    }
}