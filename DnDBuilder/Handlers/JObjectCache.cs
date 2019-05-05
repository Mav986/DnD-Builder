using System;
using System.Runtime.Caching;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Handlers
{
    public class JObjectCache
    {
        private readonly ObjectCache _cache;
        
        /// <summary>
        /// Stores and retrieves JObject's in an ObjectCache
        /// </summary>
        public JObjectCache()
        {
            _cache = MemoryCache.Default;
        }

        /// <summary>
        /// Add a JObject to the cache
        /// </summary>
        /// <param name="key">A unique string to locate the object</param>
        /// <param name="data">JObject to be stored</param>
        /// <param name="expiry">An integer indicating how many minutes an object should be cached</param>
        public void Add(string key, JObject data, int expiry)
        {
            if (Contains(key))
            {
                throw new ArgumentException("Cannot add duplicate key", key);
            }
            
            DateTimeOffset expiryInMinutes = DateTimeOffset.Now.AddMinutes(expiry);
            _cache.Add(key, data, expiryInMinutes);
        }

        /// <summary>
        /// Retrieve a JObject from the cache
        /// </summary>
        /// <param name="key">A unique string to locate the object</param>
        /// <returns>The JObject from the cache, otherwise null</returns>
        public JObject Get(string key)
        {
            return _cache.Get(key) as JObject;
        }

        /// <summary>
        /// Check if a JObject is cached
        /// </summary>
        /// <param name="key">A unique string to locate the object</param>
        /// <returns>True if the JObject is cached, otherwise False</returns>
        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }
    }
}