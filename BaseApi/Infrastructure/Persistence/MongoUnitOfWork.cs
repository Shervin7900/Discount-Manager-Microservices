using System;
using System.Threading;
using System.Threading.Tasks;
using BaseApi.Domain.Interfaces;
using MongoDB.Driver;

namespace BaseApi.Infrastructure.Persistence;

public class MongoUnitOfWork : IMongoUnitOfWork
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private IClientSessionHandle? _session;

    public MongoUnitOfWork(IMongoClient client, string databaseName)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _database = _client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

    public async Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
    {
        _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
        return _session;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session == null)
        {
            await StartSessionAsync(cancellationToken);
        }
        _session!.StartTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session != null && _session.IsInTransaction)
        {
            await _session.CommitTransactionAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session != null && _session.IsInTransaction)
        {
            await _session.AbortTransactionAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        _session?.Dispose();
        GC.SuppressFinalize(this);
    }
}
