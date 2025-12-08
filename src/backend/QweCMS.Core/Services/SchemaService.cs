using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

public class SchemaService : ISchemaService
{
    private readonly ISchemaRepository _repository;
    private readonly IMixinService _mixinService;
    private readonly IJsonSchemaValidationService _validationService;

    public SchemaService(ISchemaRepository repository, IMixinService mixinService, IJsonSchemaValidationService validationService)
    {
        _repository = repository;
        _mixinService = mixinService;
        _validationService = validationService;
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
    public async Task<OperationResult<SchemaEntity>> CreateAsync(SchemaEntity entity)
    {
        // Валидация связей с миксинами
        var mixinValidation = await ValidateMixinsAsync(entity);
        if (!mixinValidation.IsValid)
        {
            return OperationResult<SchemaEntity>.Failure(mixinValidation.Errors, "Invalid mixin references");
        }

        // Валидация JSON Schema самой схемы
        var schemaValidation = _validationService.ValidateSchema(entity.Schema);
        if (!schemaValidation.IsValid)
        {
            return OperationResult<SchemaEntity>.Failure(schemaValidation.Errors, "Invalid JSON Schema");
        }

        try
        {
            var createdEntity = await _repository.CreateAsync(entity);
            return OperationResult<SchemaEntity>.Success(createdEntity, "Schema created successfully");
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "CREATE_ERROR",
                Message = $"Failed to create schema: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<SchemaEntity>.Failure(error, "Failed to create schema");
        }
    }

    /// <inheritdoc/>
    public async Task<OperationResult<SchemaEntity>> UpdateAsync(string id, SchemaEntity entity)
    {
        // Валидация связей с миксинами
        var mixinValidation = await ValidateMixinsAsync(entity);
        if (!mixinValidation.IsValid)
        {
            return OperationResult<SchemaEntity>.Failure(mixinValidation.Errors, "Invalid mixin references");
        }

        // Валидация JSON Schema самой схемы
        var schemaValidation = _validationService.ValidateSchema(entity.Schema);
        if (!schemaValidation.IsValid)
        {
            return OperationResult<SchemaEntity>.Failure(schemaValidation.Errors, "Invalid JSON Schema");
        }

        try
        {
            var updatedEntity = await _repository.UpdateAsync(id, entity);
            return OperationResult<SchemaEntity>.Success(updatedEntity, "Schema updated successfully");
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "UPDATE_ERROR",
                Message = $"Failed to update schema: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<SchemaEntity>.Failure(error, "Failed to update schema");
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
                return OperationResult<bool>.Success(true, "Schema deleted successfully");
            }
            else
            {
                var error = new ValidationError
                {
                    Code = "NOT_FOUND",
                    Message = "Schema not found",
                    PropertyPath = "$.id"
                };
                return OperationResult<bool>.Failure(error, "Schema not found");
            }
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "DELETE_ERROR",
                Message = $"Failed to delete schema: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<bool>.Failure(error, "Failed to delete schema");
        }
    }

    /// <inheritdoc/>
    public async Task<ValidationResult> ValidateMixinsAsync(SchemaEntity entity)
    {
        var errors = new List<ValidationError>();

        if (entity.Mixins == null || !entity.Mixins.Any())
        {
            return new ValidationResult { IsValid = true };
        }

        var allMixins = await _mixinService.GetAllAsync();
        var mixinNames = allMixins.Select(m => m.Name).ToHashSet();

        foreach (var mixinRef in entity.Mixins)
        {
            if (string.IsNullOrWhiteSpace(mixinRef.Name))
            {
                errors.Add(new ValidationError
                {
                    Code = "EMPTY_MIXIN_NAME",
                    Message = "Mixin name cannot be empty",
                    PropertyPath = "$.mixins"
                });
                continue;
            }

            if (!mixinNames.Contains(mixinRef.Name))
            {
                errors.Add(new ValidationError
                {
                    Code = "MIXIN_NOT_FOUND",
                    Message = $"Mixin '{mixinRef.Name}' not found",
                    PropertyPath = "$.mixins"
                });
            }

            if (string.IsNullOrWhiteSpace(mixinRef.Path))
            {
                errors.Add(new ValidationError
                {
                    Code = "EMPTY_MIXIN_PATH",
                    Message = $"Path for mixin '{mixinRef.Name}' cannot be empty",
                    PropertyPath = "$.mixins"
                });
            }
        }

        return errors.Any()
            ? new ValidationResult { IsValid = false, Errors = errors }
            : new ValidationResult { IsValid = true };
    }

    /// <inheritdoc/>
    public async Task<OperationResult<object>> GetComposedSchemaAsync(string id)
    {
        var schema = await GetByIdAsync(id);
        if (schema == null)
        {
            var error = new ValidationError
            {
                Code = "NOT_FOUND",
                Message = "Schema not found",
                PropertyPath = "$.id"
            };
            return OperationResult<object>.Failure(error, "Schema not found");
        }

        try
        {
            // Базовая реализация - просто возвращаем схему
            // В будущем здесь будет логика компоновки с миксинами
            return OperationResult<object>.Success(schema.Schema, "Composed schema retrieved successfully");
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "COMPOSITION_ERROR",
                Message = $"Failed to compose schema: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<object>.Failure(error, "Failed to compose schema");
        }
    }

    /// <inheritdoc/>
    public async Task<PagedResult<SchemaEntity>> GetPagedAsync(SearchParameters parameters)
    {
        return await _repository.GetPagedAsync(parameters);
    }
}