using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Web
{
    public class RequestHandler
    {
        private readonly CacheHandler _cacheHandler;
        private readonly HttpClient _client;

        /// <summary>
        ///     Handler for making http requests
        /// </summary>
        /// <param name="url">A string containing the base url to make requests to</param>
        /// <param name="cacheHandler">A CacheHandler object</param>
        public RequestHandler(string url, CacheHandler cacheHandler)
        {
            _cacheHandler = cacheHandler;
            _client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        /// <summary>
        ///     Get a JObject from the cache or remote server
        /// </summary>
        /// <param name="key">A unique key to locate a cache item</param>
        /// <param name="url">A url to call if item is not in the cache</param>
        /// <returns>The requested JObject</returns>
        public JObject GetFromCache(string key, string url)
        {
            JObject res;

            if (_cacheHandler.Contains(key))
            {
                res = _cacheHandler.Get(key);
            }
            else
            {
                res = GetJson(url);
                _cacheHandler.Add(key, res);
            }

            return res;
        }

        /// <summary>
        ///     Extract a child JObject from a JToken
        /// </summary>
        /// <param name="token">JToken to extract JObject from</param>
        /// <param name="name">Name of the JObject to be extracted</param>
        /// <returns>The specified JObject</returns>
        /// <exception cref="ArgumentException">Thrown when the JObject does not exist in the JToken</exception>
        public JObject ExtractFromJArray(JToken token, string name)
        {
            var json = token.Children<JObject>()
                .FirstOrDefault(r => r["name"].ToString().Equals(name, StringComparison.OrdinalIgnoreCase));

            if (json == null) throw new ArgumentException($"{name} not found");

            var key = json["name"].ToString();
            var url = json["url"].ToString();

            return GetFromCache(key, url);
        }

        /// <summary>
        ///     Request a JObject from an endpoint
        /// </summary>
        /// <param name="url">A string representing the url to get JSON from</param>
        /// <returns>A JObject of the response body JSON</returns>
        private JObject GetJson(string url)
        {
            var endpoint = ExtractEndpoint(url);
            var res = _client.GetAsync(endpoint).Result;
            res.EnsureSuccessStatusCode();
            var json = res.Content.ReadAsAsync<JObject>().Result;

            return json;
        }

        /// <summary>
        ///     Extract an endpoint from the base url
        /// </summary>
        /// <param name="url">A base url of the form "domain.com/api/some-endpoint-route</param>
        /// <returns>The endpoint extracted from the base url</returns>
        private string ExtractEndpoint(string url)
        {
            var baseUrl = _client.BaseAddress.ToString();
            if (!url.StartsWith(baseUrl)) return url;
            var result = url.Split(new[] {baseUrl}, StringSplitOptions.None);

            return string.Join("", result);
        }
    }
}