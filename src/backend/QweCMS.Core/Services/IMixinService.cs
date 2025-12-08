using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

public interface IMixinRepository
{
    Task<IEnumerable<MixinEntity>> GetAllAsync();
    Task<MixinEntity?> GetByIdAsync(string id);
    Task<MixinEntity> CreateAsync(MixinEntity entity);
    Task<MixinEntity> UpdateAsync(string id, MixinEntity entity);
    Task<bool> DeleteAsync(string id);
    Task<PagedResult<MixinEntity>> GetPagedAsync(SearchParameters parameters);
}

public interface IMixinService
{
    Task<IEnumerable<MixinEntity>> GetAllAsync();
    Task<MixinEntity?> GetByIdAsync(string id);
    Task<MixinEntity> CreateAsync(MixinEntity entity);
    Task<MixinEntity> UpdateAsync(string id, MixinEntity entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> ValidateSchemaAsync(object schema);
    Task<PagedResult<MixinEntity>> GetPagedAsync(SearchParameters parameters);
}