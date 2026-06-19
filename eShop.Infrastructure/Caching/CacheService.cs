using eShop.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace eShop.Infrastructure.Caching
{
    public class CacheService(IDistributedCache distributedCache, IConnectionMultiplexer redis) : ICacheService
    {
        private readonly IDatabase _database = redis.GetDatabase();

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            string? cachedData = await distributedCache.GetStringAsync(key);
            if (cachedData is null) return null;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);

            await distributedCache.SetStringAsync(
                key,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                });
        }

        public async Task RemoveAsync(string key)
        {
            await distributedCache.RemoveAsync(key);
        }

        public async Task<bool> ReserveStockAsync(string key, int quantity)
        {
            string luaScript = @"
                local current = redis.call('get', KEYS[1])
                if not current then
                    return 0
                end
                if tonumber(current) >= tonumber(ARGV[1]) then
                    return redis.call('decrby', KEYS[1], ARGV[1])
                else
                    return -1
                end";

            var result = (long)await _database.ScriptEvaluateAsync(luaScript, [key], [quantity]);

            return result >= 0;
        }

        public async Task IncrementStockAsync(string key, int quantity)
        {
            // StringIncrementAsync uses the native Redis INCRBY command, which is natively atomic.
            await _database.StringIncrementAsync(key, quantity);
        }
    }
}
