using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BaseApi.Domain.Interfaces;

public interface IMongoUnitOfWork : IDisposable
{
    IMongoCollection<T> GetCollection<T>(string name);
    Task<T?> GetByIdAsync<T>(string collectionName, object id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync<T>(string collectionName, FilterDefinition<T> filter, CancellationToken cancellationToken = default);
    Task AddAsync<T>(string collectionName, T entity, CancellationToken cancellationToken = default);
    Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
