using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using DemoApi.Dtos;
using StackExchange.Redis;

namespace DemoApi.Services
{
    public interface ICacheService
    {
        Task<Dictionary<string, MyCacheData>> GetCacheDatasAsync();
        Task<MyCacheData?> GetCacheDataAsync(string key);
        Task UpdateCacheDataAsync(string key, MyCacheData cacheData);
        Task DeleteCacheDataAsync(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly TimeSpan _expirationTime;
        private readonly string _redisConnectionString;

        public CacheService(IDistributedCache distributedCache, TimeSpan expirationTime, string redisConnectionString)
        {
            _distributedCache = distributedCache;
            _expirationTime = expirationTime;
            _redisConnectionString = redisConnectionString;
        }

        public async Task<Dictionary<string, MyCacheData>> GetCacheDatasAsync()
        {
            var cacheDatas = new Dictionary<string, MyCacheData>();
            
            var redis = ConnectionMultiplexer.Connect(_redisConnectionString);
            var database = redis.GetDatabase();

            var server = redis.GetServer(redis.GetEndPoints().First());

            foreach (var key in server.Keys(pattern: "DemoApi-*"))
            {
                var suffix = key.ToString().Substring("DemoApi-".Length);

                var keyType = await database.KeyTypeAsync(key);

                switch (keyType)
                {
                    case RedisType.Hash:
                        var value = await GetCacheDataAsync(suffix);
                        cacheDatas[suffix] = value;
                        break;
                    // Add other cases if needed for different types like hashes, lists, sets, etc.
                    default:
                        // Handle or log the case where the key has an unexpected type
                        break;
                }
            }

            return cacheDatas;
        }

        public async Task<MyCacheData?> GetCacheDataAsync(string key)
        {
            // Get from Redis Cache
            if (string.IsNullOrEmpty(key)) return null;

            var json = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(json)) return null;

            var myCacheData = JsonSerializer.Deserialize<MyCacheData>(json);

            return myCacheData;
        }

        public async Task UpdateCacheDataAsync(string key, MyCacheData cacheData)
        {
            // Save to Redis Cache
            if (string.IsNullOrEmpty(key)) return;

            var json = JsonSerializer.Serialize(cacheData, new JsonSerializerOptions() { WriteIndented = true });

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _expirationTime
            };

            await _distributedCache.SetStringAsync(key, json, options);
        }

        public async Task DeleteCacheDataAsync(string key)
        {
            // Delete from Redis Cache
            if (string.IsNullOrEmpty(key)) return;

            await _distributedCache.RemoveAsync(key);
        }
    }
}

