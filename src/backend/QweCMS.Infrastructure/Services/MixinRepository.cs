using QweCMS.Core.Entities;
using QweCMS.Core.Services;
using QweCMS.Core.Models;
using QweCMS.Infrastructure.Repositories;
using MongoDB.Driver;

namespace QweCMS.Infrastructure.Services;

public class MixinRepository : IMixinRepository
{
    private readonly IMongoRepository<MixinEntity> _repository;

    public MixinRepository(IMongoRepository<MixinEntity> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MixinEntity>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<MixinEntity?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<MixinEntity> CreateAsync(MixinEntity entity)
    {
        return await _repository.CreateAsync(entity);
    }

    public async Task<MixinEntity> UpdateAsync(string id, MixinEntity entity)
    {
        return await _repository.UpdateAsync(id, entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<PagedResult<MixinEntity>> GetPagedAsync(SearchParameters parameters)
    {
        var filter = BuildFilter(parameters);
        var sort = BuildSort(parameters);
        
        var totalCount = await _repository.CountAsync(filter);
        var skip = (parameters.Page - 1) * parameters.PageSize;
        
        var pagedData = await _repository.FindAsync(filter, sort, skip, parameters.PageSize);
            
        return new PagedResult<MixinEntity>
        {
            Data = pagedData,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = (int)totalCount
        };
    }

    private FilterDefinition<MixinEntity> BuildFilter(SearchParameters parameters)
    {
        var builder = Builders<MixinEntity>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            filter = builder.Or(
                builder.Regex(m => m.Name, $"(?i){searchTerm}"),
                builder.Regex(m => m.DisplayName, $"(?i){searchTerm}"),
                builder.Regex(m => m.Description, $"(?i){searchTerm}")
            );
        }

        return filter;
    }

    private SortDefinition<MixinEntity>? BuildSort(SearchParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.SortBy))
            return null;

        var builder = Builders<MixinEntity>.Sort;
        
        return parameters.SortDescending 
            ? builder.Descending(parameters.SortBy)
            : builder.Ascending(parameters.SortBy);
    }
}