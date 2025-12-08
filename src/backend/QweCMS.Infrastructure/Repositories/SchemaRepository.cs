using MongoDB.Driver;
using QweCMS.Core.Entities;
using QweCMS.Core.Models;
using QweCMS.Core.Services;

namespace QweCMS.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы со схемами в MongoDB
/// </summary>
public class SchemaRepository : ISchemaRepository
{
    private readonly IMongoRepository<SchemaEntity> _repository;

    public SchemaRepository(IMongoRepository<SchemaEntity> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SchemaEntity>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <inheritdoc/>
    public async Task<SchemaEntity?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <inheritdoc/>
    public async Task<SchemaEntity> CreateAsync(SchemaEntity entity)
    {
        return await _repository.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public async Task<SchemaEntity> UpdateAsync(string id, SchemaEntity entity)
    {
        return await _repository.UpdateAsync(id, entity);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<SchemaEntity>> GetPagedAsync(SearchParameters parameters)
    {
        var filter = Builders<SchemaEntity>.Filter.Empty;
        var sort = Builders<SchemaEntity>.Sort.Descending(x => x.CreatedAt);

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            filter = Builders<SchemaEntity>.Filter.Or(
                Builders<SchemaEntity>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(parameters.SearchTerm, "i")),
                Builders<SchemaEntity>.Filter.Regex(x => x.DisplayName, new MongoDB.Bson.BsonRegularExpression(parameters.SearchTerm, "i")),
                Builders<SchemaEntity>.Filter.Regex(x => x.Description, new MongoDB.Bson.BsonRegularExpression(parameters.SearchTerm, "i"))
            );
        }

        var totalCount = await _repository.CountAsync(filter);
        var skip = (parameters.Page - 1) * parameters.PageSize;
        var items = await _repository.FindAsync(filter, sort, skip, parameters.PageSize);

        return new PagedResult<SchemaEntity>
        {
            Data = items,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = (int)totalCount
        };
    }
}