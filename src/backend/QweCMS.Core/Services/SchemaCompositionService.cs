using System.Collections.Concurrent;
using System.Text.Json;
using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

/// <summary>
/// Сервис для компоновки JSON Schema из миксинов
/// </summary>
public class SchemaCompositionService : ISchemaCompositionService
{
    private readonly IMixinService _mixinService;
    private readonly JsonPointerService _jsonPointerService;
    private readonly ConcurrentDictionary<string, (object Schema, DateTime Timestamp)> _cache = new();

    public SchemaCompositionService(IMixinService mixinService, JsonPointerService jsonPointerService)
    {
        _mixinService = mixinService;
        _jsonPointerService = jsonPointerService;
    }

    /// <inheritdoc/>
    public async Task<OperationResult<object>> ComposeSchemaAsync(SchemaEntity schema)
    {
        try
        {
            // Проверяем кеш
            var cacheKey = schema.Id;
            if (_cache.TryGetValue(cacheKey, out var cached) &&
                cached.Timestamp > schema.UpdatedAt)
            {
                return OperationResult<object>.Success(cached.Schema, "Schema retrieved from cache");
            }

            // Валидируем пути миксинов
            var pathValidation = await ValidateMixinPathsAsync(schema);
            if (!pathValidation.IsValid)
            {
                return OperationResult<object>.Failure(pathValidation.Errors, "Invalid mixin paths");
            }

            // Проверяем циклические зависимости
            var cycleValidation = await CheckCircularDependenciesAsync(schema);
            if (!cycleValidation.IsValid)
            {
                return OperationResult<object>.Failure(cycleValidation.Errors, "Circular dependencies detected");
            }

            // Получаем все миксины
            var allMixins = await _mixinService.GetAllAsync();
            var mixinDict = allMixins.ToDictionary(m => m.Name);

            // Начинаем с базовой схемы
            var composedSchema = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(schema.Schema));

            // Компонуем миксины
            foreach (var mixinRef in schema.Mixins)
            {
                if (!mixinDict.TryGetValue(mixinRef.Name, out var mixin))
                {
                    var error = new ValidationError
                    {
                        Code = "MIXIN_NOT_FOUND",
                        Message = $"Mixin '{mixinRef.Name}' not found",
                        PropertyPath = "$.mixins"
                    };
                    return OperationResult<object>.Failure(error, "Mixin not found");
                }

                var mixinSchema = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(mixin.Schema));
                composedSchema = MergeSchemas(composedSchema, mixinSchema, mixinRef.Path);
            }

            // Кешируем результат
            var result = JsonSerializer.Deserialize<object>(composedSchema.GetRawText());
            _cache[cacheKey] = (result!, DateTime.UtcNow);

            return OperationResult<object>.Success(result!, "Schema composed successfully");
        }
        catch (Exception ex)
        {
            var error = new ValidationError
            {
                Code = "COMPOSITION_ERROR",
                Message = $"Failed to compose schema: {ex.Message}",
                PropertyPath = "$"
            };
            return OperationResult<object>.Failure(error, "Schema composition failed");
        }
    }

    /// <inheritdoc/>
    public Task<ValidationResult> ValidateMixinPathsAsync(SchemaEntity schema)
    {
        var errors = new List<ValidationError>();

        if (schema.Mixins == null || !schema.Mixins.Any())
            return Task.FromResult(new ValidationResult { IsValid = true });

        foreach (var mixinRef in schema.Mixins)
        {
            // Проверяем валидность JSON Pointer
            if (!_jsonPointerService.IsValidPointer(mixinRef.Path))
            {
                errors.Add(new ValidationError
                {
                    Code = "INVALID_JSON_POINTER",
                    Message = $"Invalid JSON Pointer path: '{mixinRef.Path}' for mixin '{mixinRef.Name}'",
                    PropertyPath = "$.mixins"
                });
            }

            // Для нашей системы допускаем только пути, начинающиеся с "$"
            if (!mixinRef.Path.StartsWith("$"))
            {
                errors.Add(new ValidationError
                {
                    Code = "INVALID_PATH_ROOT",
                    Message = $"Mixin path must start with '$' for mixin '{mixinRef.Name}'",
                    PropertyPath = "$.mixins"
                });
            }
        }

        return Task.FromResult(errors.Any()
            ? new ValidationResult { IsValid = false, Errors = errors }
            : new ValidationResult { IsValid = true });
    }

    /// <inheritdoc/>
    public Task<ValidationResult> CheckCircularDependenciesAsync(SchemaEntity schema)
    {
        // Для простоты - базовая проверка, что схема не ссылается сама на себя
        // В будущем можно расширить для проверки цепочек зависимостей
        var errors = new List<ValidationError>();

        if (schema.Mixins != null)
        {
            var mixinNames = schema.Mixins.Select(m => m.Name).ToHashSet();
            if (mixinNames.Contains(schema.Name))
            {
                errors.Add(new ValidationError
                {
                    Code = "SELF_REFERENCE",
                    Message = "Schema cannot reference itself as a mixin",
                    PropertyPath = "$.mixins"
                });
            }
        }

        return Task.FromResult(errors.Any()
            ? new ValidationResult { IsValid = false, Errors = errors }
            : new ValidationResult { IsValid = true });
    }

    /// <inheritdoc/>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Сливает две JSON схемы по указанному пути
    /// </summary>
    private JsonElement MergeSchemas(JsonElement baseSchema, JsonElement mixinSchema, string path)
    {
        if (path == "$")
        {
            // Полная замена корневой схемы
            return DeepMerge(baseSchema, mixinSchema);
        }
        else
        {
            // Вставка в указанный путь
            return _jsonPointerService.SetValue(baseSchema, path, mixinSchema);
        }
    }

    /// <summary>
    /// Глубокое слияние двух JSON объектов
    /// </summary>
    private JsonElement DeepMerge(JsonElement target, JsonElement source)
    {
        if (target.ValueKind != JsonValueKind.Object || source.ValueKind != JsonValueKind.Object)
        {
            return source; // Source заменяет target
        }

        var result = new Dictionary<string, JsonElement>();

        // Копируем все из target
        foreach (var property in target.EnumerateObject())
        {
            result[property.Name] = property.Value;
        }

        // Сливаем из source
        foreach (var property in source.EnumerateObject())
        {
            if (result.ContainsKey(property.Name))
            {
                // Рекурсивное слияние если оба объекта
                if (result[property.Name].ValueKind == JsonValueKind.Object && property.Value.ValueKind == JsonValueKind.Object)
                {
                    result[property.Name] = DeepMerge(result[property.Name], property.Value);
                }
                else
                {
                    // Source перезаписывает target
                    result[property.Name] = property.Value;
                }
            }
            else
            {
                result[property.Name] = property.Value;
            }
        }

        return JsonSerializer.SerializeToElement(result);
    }
}