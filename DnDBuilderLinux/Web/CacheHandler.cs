using System;
using System.Runtime.Caching;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Web
{
    public class CacheHandler
    {
        private const int CacheExpiryInMinutes = 60;
        private readonly ObjectCache _cache;

        /// <summary>
        ///     Stores and retrieves JObject's in an ObjectCache
        /// </summary>
        public CacheHandler()
        {
            _cache = MemoryCache.Default;
        }

        /// <summary>
        ///     Add a JObject to the cache
        /// </summary>
        /// <param name="key">A unique string to locate the object</param>
        /// <param name="data">JObject to be stored</param>
        public void Add(string key, JObject data)
        {
            if (Contains(key)) throw new ArgumentException("Cannot add duplicate key", key);

            var expiryInMinutes = DateTimeOffset.Now.AddMinutes(CacheExpiryInMinutes);
            _cache.Add(key, data, expiryInMinutes);
        }

        /// <summary>
        ///     Retrieve a JObject from the cache
        /// </summary>
        /// <param name="key">A unique string to locate the object</param>
        /// <returns>The JObject from the cache, otherwise null</returns>
        public JObject Get(string key)
        {
            return _cache.Get(key) as JObject;
        }

        /// <summary>
        ///     Check if a JObject is cached
        /// </summary>
        /// <param name="key">A unique string to locate the object</param>
        /// <returns>True if the JObject is cached, otherwise False</returns>
        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }
    }
}