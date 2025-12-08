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
    public async Task<OperationResult<MixinEntity>> CreateAsync(MixinEntity entity)
    {
        var validationResult = _validationService.ValidateSchema(entity.Schema);
        if (!validationResult.IsValid)
        {
            return OperationResult<MixinEntity>.Failure(validationResult.Errors, "Invalid JSON Schema");
        }

        try
        {
            var createdEntity = await _repository.CreateAsync(entity);
            return OperationResult<MixinEntity>.Success(createdEntity, "Mixin created successfully");
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "CREATE_ERROR",
                Message = $"Failed to create mixin: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<MixinEntity>.Failure(error, "Failed to create mixin");
        }
    }

    /// <inheritdoc/>
    public async Task<OperationResult<MixinEntity>> UpdateAsync(string id, MixinEntity entity)
    {
        var validationResult = _validationService.ValidateSchema(entity.Schema);
        if (!validationResult.IsValid)
        {
            return OperationResult<MixinEntity>.Failure(validationResult.Errors, "Invalid JSON Schema");
        }

        try
        {
            var updatedEntity = await _repository.UpdateAsync(id, entity);
            return OperationResult<MixinEntity>.Success(updatedEntity, "Mixin updated successfully");
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "UPDATE_ERROR",
                Message = $"Failed to update mixin: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<MixinEntity>.Failure(error, "Failed to update mixin");
        }
    }

    /// <inheritdoc/>
    public async Task<OperationResult<bool>> DeleteAsync(string id)
    {
        try
        {
            var result = await _repository.DeleteAsync(id);
            if (result)
            {
                return OperationResult<bool>.Success(true, "Mixin deleted successfully");
            }
            else
            {
                var error = new ValidationError
                {
                    Code = "NOT_FOUND",
                    Message = "Mixin not found",
                    PropertyPath = "$.id"
                };
                return OperationResult<bool>.Failure(error, "Mixin not found");
            }
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "DELETE_ERROR",
                Message = $"Failed to delete mixin: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<bool>.Failure(error, "Failed to delete mixin");
        }
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