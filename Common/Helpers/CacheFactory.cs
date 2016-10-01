using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CacheFactory
    {
        private MemoryCache _cache;

        public ObjectCache CacheInstance
        {
            get
            {
                return _cache;
            }
        }

        public CacheFactory()
        {

            _cache = MemoryCache.Default;
        }
        public async Task<object> GetCacheAsync(string key)
        {
            return await Task.Run(() =>
            {
                var data = _cache.Get(key);
                return data;
            });
        }


        public object GetCache(string key)
        {

            var data = _cache.Get(key);
            return data;

        }
        public void SaveCache(string key, object data)
        {
            _cache.Set(key, data, GetCachePolicy());
        }
        public void SaveCache(string key, object data, CacheItemPolicy policy)
        {
            _cache.Set(key, data, policy);
        }
        public object RemoveCache(string key)
        {
            var removeObject = _cache.Remove(key);
            return removeObject;
        }
        public CacheItemPolicy GetCachePolicy()
        {
            var p = new CacheItemPolicy();
            return p;
        }
    }
}
