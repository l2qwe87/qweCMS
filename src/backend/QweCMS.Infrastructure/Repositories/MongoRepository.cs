using MongoDB.Driver;
using MongoDB.Driver;
using QweCMS.Core.Entities;

namespace QweCMS.Infrastructure.Repositories;

/// <summary>
/// Общий интерфейс репозитория для работы с MongoDB
/// </summary>
/// <typeparam name="T">Тип сущности, наследуемый от BaseEntity</typeparam>
public interface IMongoRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Получить все документы коллекции
    /// </summary>
    /// <returns>Коллекция всех документов</returns>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>
    /// Получить документ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    /// <returns>Документ или null если не найден</returns>
    Task<T?> GetByIdAsync(string id);
    
    /// <summary>
    /// Создать новый документ
    /// </summary>
    /// <param name="entity">Сущность для создания</param>
    /// <returns>Созданная сущность</returns>
    Task<T> CreateAsync(T entity);
    
    /// <summary>
    /// Обновить существующий документ
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    /// <param name="entity">Обновленная сущность</param>
    /// <returns>Обновленная сущность</returns>
    Task<T> UpdateAsync(string id, T entity);
    
    /// <summary>
    /// Удалить документ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    /// <returns>True если удален, иначе false</returns>
    Task<bool> DeleteAsync(string id);
    
    /// <summary>
    /// Найти документы по фильтру
    /// </summary>
    /// <param name="filter">Фильтр поиска</param>
    /// <returns>Коллекция найденных документов</returns>
    Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter);
    
    /// <summary>
    /// Найти документы с пагинацией и сортировкой
    /// </summary>
    /// <param name="filter">Фильтр поиска</param>
    /// <param name="sort">Параметры сортировки</param>
    /// <param name="skip">Количество пропускаемых документов</param>
    /// <param name="limit">Максимальное количество документов</param>
    /// <returns>Коллекция найденных документов</returns>
    Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter, SortDefinition<T>? sort = null, int? skip = null, int? limit = null);
    
    /// <summary>
    /// Подсчитать количество документов по фильтру
    /// </summary>
    /// <param name="filter">Фильтр подсчета</param>
    /// <returns>Количество документов</returns>
    Task<long> CountAsync(FilterDefinition<T> filter);
}

public class MongoRepository<T> : IMongoRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<T> CreateAsync(T entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    /// <inheritdoc/>
    public async Task<T> UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _collection.ReplaceOneAsync(filter, entity);
        return entity;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<long> CountAsync(FilterDefinition<T> filter)
    {
        return await _collection.CountDocumentsAsync(filter);
    }
}