using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

public class MixinService : IMixinService
{
    private readonly IMixinRepository _repository;

    public MixinService(IMixinRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MixinEntity>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <inheritdoc/>
    public async Task<MixinEntity?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <inheritdoc/>
    public async Task<MixinEntity> CreateAsync(MixinEntity entity)
    {
        if (!ValidateSchema(entity.Schema))
        {
            throw new ArgumentException("Invalid JSON Schema");
        }

        return await _repository.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public async Task<MixinEntity> UpdateAsync(string id, MixinEntity entity)
    {
        if (!ValidateSchema(entity.Schema))
        {
            throw new ArgumentException("Invalid JSON Schema");
        }

        return await _repository.UpdateAsync(id, entity);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public Task<bool> ValidateSchemaAsync(object schema)
    {
        return Task.FromResult(ValidateSchema(schema));
    }

    /// <inheritdoc/>
    public async Task<PagedResult<MixinEntity>> GetPagedAsync(SearchParameters parameters)
    {
        return await _repository.GetPagedAsync(parameters);
    }

    private bool ValidateSchema(object schema)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(schema);
            var document = System.Text.Json.JsonDocument.Parse(json);
            
            if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                return false;
            }

            if (!document.RootElement.TryGetProperty("type", out var typeProperty) ||
                typeProperty.GetString() != "object")
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}