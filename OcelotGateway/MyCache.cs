using Ocelot.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OcelotGateway
{
    public class MyCache : IOcelotCache<CachedResponse>
    {
        private static Dictionary<string, CacheObj> _cacheObjs = new Dictionary<string, CacheObj>();

        public void Add(string key, CachedResponse value, TimeSpan ttl, string region)
        {
            if (!_cacheObjs.ContainsKey($"{region}_{key}"))
            {
                _cacheObjs.Add($"{region}_{key}", new CacheObj
                {
                    ExpireTime = DateTime.Now.Add(ttl),
                    Response = value
                });
            }
        }

        public CachedResponse Get(string key, string region)
        {
            if (!_cacheObjs.ContainsKey($"{region}_{key}")) return null;

            var cacheObj = _cacheObjs[$"{region}_{key}"];
            if (cacheObj != null && cacheObj.ExpireTime > DateTime.Now)
            {
                return cacheObj.Response;
            }

            _cacheObjs.Remove($"{region}_{key}");
            return null;
        }

        public void ClearRegion(string region)
        {
            var cacheItemKeys = _cacheObjs.Where(s => s.Key.StartsWith($"{region}_"))
                .Select(s => s.Key);

            foreach (var key in cacheItemKeys)
            {
                _cacheObjs.Remove(key);
            }
        }

        public void AddAndDelete(string key, CachedResponse value, TimeSpan ttl, string region)
        {
            if (_cacheObjs.ContainsKey($"{region}_{key}"))
            {
                _cacheObjs.Remove($"{region}_{key}");
            }

            _cacheObjs.Add($"{region}_{key}", new CacheObj
            {
                ExpireTime = DateTime.Now.Add(ttl),
                Response = value
            });
        }

        internal class CacheObj
        {
            internal DateTime ExpireTime { get; set; }
            internal CachedResponse Response { get; set; }
        }
    }
}