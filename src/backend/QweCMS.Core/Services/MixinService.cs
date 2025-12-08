using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

public class MixinService : IMixinService
{
    private readonly IMixinRepository _repository;
    private readonly IJsonSchemaValidationService _validationService;

    public MixinService(IMixinRepository repository, IJsonSchemaValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
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
        var validationResult = _validationService.ValidateSchema(entity.Schema);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.Message));
            throw new ArgumentException($"Invalid JSON Schema: {errorMessage}");
        }

        return await _repository.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public async Task<MixinEntity> UpdateAsync(string id, MixinEntity entity)
    {
        var validationResult = _validationService.ValidateSchema(entity.Schema);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.Message));
            throw new ArgumentException($"Invalid JSON Schema: {errorMessage}");
        }

        return await _repository.UpdateAsync(id, entity);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public Task<ValidationResult> ValidateSchemaAsync(object schema)
    {
        var result = _validationService.ValidateSchema(schema);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<MixinEntity>> GetPagedAsync(SearchParameters parameters)
    {
        return await _repository.GetPagedAsync(parameters);
    }


}