using System;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Web
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
        ///     Request a JObject from an endpoint
        /// </summary>
        /// <param name="url">A string representing the url to get JSON from</param>
        /// <returns>A JObject of the response body JSON</returns>
        private JObject GetJson(string url)
        {
            
            string endpoint = ExtractEndpoint(url);
            HttpResponseMessage res = _client.GetAsync(HttpUtility.UrlPathEncode(endpoint)).Result;
            res.EnsureSuccessStatusCode();
            JObject json = res.Content.ReadAsAsync<JObject>().Result;

            return json;
        }

        /// <summary>
        ///     Extract an endpoint from the base url
        /// </summary>
        /// <param name="url">A base url of the form "domain.com/api/some-endpoint-route</param>
        /// <returns>The endpoint extracted from the base url</returns>
        private string ExtractEndpoint(string url)
        {
            string baseUrl = _client.BaseAddress.ToString();
            if (!url.StartsWith(baseUrl)) return url;
            string[] result = url.Split(new[] {baseUrl}, StringSplitOptions.None);

            return string.Join("", result);
        }
    }
}