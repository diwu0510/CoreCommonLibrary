using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

namespace HZC.Utils.Mvcs
{
    /// <summary>
    /// 分布式缓存扩展
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="option"></param>
        public static void Set<T>(this IDistributedCache cache, string key, T obj, DistributedCacheEntryOptions option = null)
        {
            option = option ?? new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(20) };
            cache.SetString(key, JsonConvert.SerializeObject(obj), option);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var str = cache.GetString(key);

            if(string.IsNullOrWhiteSpace(str))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
