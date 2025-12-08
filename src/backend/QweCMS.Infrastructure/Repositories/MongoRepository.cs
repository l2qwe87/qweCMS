using MongoDB.Driver;
using QweCMS.Core.Entities;

namespace QweCMS.Infrastructure.Repositories;

public interface IMongoRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(string id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter);
    Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter, SortDefinition<T>? sort = null, int? skip = null, int? limit = null);
    Task<long> CountAsync(FilterDefinition<T> filter);
}

public class MongoRepository<T> : IMongoRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<T> UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _collection.ReplaceOneAsync(filter, entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter, SortDefinition<T>? sort = null, int? skip = null, int? limit = null)
    {
        var query = _collection.Find(filter);
        
        if (sort != null)
            query = query.Sort(sort);
        
        if (skip.HasValue)
            query = query.Skip(skip.Value);
        
        if (limit.HasValue)
            query = query.Limit(limit.Value);
        
        return await query.ToListAsync();
    }

    public async Task<long> CountAsync(FilterDefinition<T> filter)
    {
        return await _collection.CountDocumentsAsync(filter);
    }
}