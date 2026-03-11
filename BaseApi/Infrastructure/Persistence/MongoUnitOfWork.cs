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

    public async Task<T?> GetByIdAsync<T>(string collectionName, object id, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", id);
        return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync<T>(string collectionName, FilterDefinition<T> filter, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<T>(collectionName);
        return await collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task AddAsync<T>(string collectionName, T entity, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<T>(collectionName);
        if (_session != null)
        {
            await collection.InsertOneAsync(_session, entity, cancellationToken: cancellationToken);
        }
        else
        {
            await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }
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
