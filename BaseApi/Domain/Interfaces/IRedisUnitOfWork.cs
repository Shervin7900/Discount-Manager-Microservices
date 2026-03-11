using System;
using System.Threading.Tasks;

namespace BaseApi.Domain.Interfaces;

public interface IRedisUnitOfWork
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> acquire, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}
