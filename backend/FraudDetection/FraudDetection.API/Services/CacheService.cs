using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace FraudDetection.API.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);

        if (data == null)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(data);
    }
    public async Task setAsync<T>(string key, T value, TimeSpan expiry)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        var data = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key,data,options);
    }
    
}