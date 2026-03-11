using System;
using System.Text.Json;
using System.Threading.Tasks;
using BaseApi.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BaseApi.Infrastructure.Persistence;

public class RedisUnitOfWork : IRedisUnitOfWork
{
    private readonly IDistributedCache _cache;

    public RedisUnitOfWork(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }

        var data = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, data, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var data = await _cache.GetAsync(key);
        return data != null;
    }
}
