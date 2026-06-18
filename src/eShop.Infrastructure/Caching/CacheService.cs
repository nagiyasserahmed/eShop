using eShop.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace eShop.Infrastructure.Caching
{
    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            string? cachedData = await distributedCache.GetStringAsync(key);
            if (cachedData is null) return null;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            string jsonData = JsonSerializer.Serialize(value);
            await distributedCache.SetStringAsync(key, jsonData, options);
        }

        public async Task RemoveAsync(string key)
        {
            await distributedCache.RemoveAsync(key);
        }
    }
}
