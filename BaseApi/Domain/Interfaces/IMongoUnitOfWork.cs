using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BaseApi.Domain.Interfaces;

public interface IMongoUnitOfWork : IDisposable
{
    IMongoCollection<T> GetCollection<T>(string name);
    Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
