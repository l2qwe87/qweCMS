using System.Text.Json;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

/// <summary>
/// Сервис валидации JSON Schema с детальной диагностикой
/// </summary>
public class JsonSchemaValidationService : IJsonSchemaValidationService
{
    /// <summary>
    /// Валидировать JSON Schema
    /// </summary>
    /// <param name="schema">Схема для валидации</param>
    /// <returns>Результат валидации с ошибками и предупреждениями</returns>
    public ValidationResult ValidateSchema(object schema)
    {
        var result = new ValidationResult();

        try
        {
            // Базовая проверка на null
            if (schema == null)
            {
                result.Errors.Add(new ValidationError
                {
                    Code = "SCHEMA_NULL",
                    Message = "Схема не может быть null",
                    PropertyPath = "$",
                    InvalidValue = null
                });
                return result;
            }

            // Преобразуем в JSON для анализа
            var jsonSchema = JsonSerializer.Serialize(schema);
            return ValidateSchemaJson(jsonSchema);
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ValidationError
            {
                Code = "SCHEMA_SERIALIZATION_ERROR",
                Message = $"Ошибка при сериализации схемы: {ex.Message}",
                PropertyPath = "$",
                InvalidValue = schema
            });
        }

        return result;
    }

    /// <summary>
    /// Валидировать JSON Schema по строке
    /// </summary>
    /// <param name="schemaJson">Схема в формате JSON строки</param>
    /// <returns>Результат валидации с ошибками и предупреждениями</returns>
    public ValidationResult ValidateSchemaJson(string schemaJson)
    {
        var result = new ValidationResult();

        try
        {
            if (string.IsNullOrWhiteSpace(schemaJson))
            {
                result.Errors.Add(new ValidationError
                {
                    Code = "SCHEMA_EMPTY",
                    Message = "Схема не может быть пустой",
                    PropertyPath = "$",
                    InvalidValue = schemaJson
                });
                return result;
            }

            // Проверяем валидность JSON
            using var document = JsonDocument.Parse(schemaJson);
            var root = document.RootElement;

            // Базовые проверки для JSON Schema
            ValidateBasicSchemaStructure(root, result);
            ValidateCommonFields(root, result);
            ValidateAdvancedRules(root, result);

            result.IsValid = result.Errors.Count == 0;
        }
        catch (JsonException ex)
        {
            result.Errors.Add(new ValidationError
            {
                Code = "INVALID_JSON",
                Message = $"Невалидный JSON: {ex.Message}",
                PropertyPath = "$",
                InvalidValue = schemaJson
            });
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ValidationError
            {
                Code = "VALIDATION_ERROR",
                Message = $"Ошибка валидации: {ex.Message}",
                PropertyPath = "$",
                InvalidValue = schemaJson
            });
        }

        return result;
    }

    /// <summary>
    /// Валидировать данные по схеме
    /// </summary>
    /// <param name="schema">Схема для валидации</param>
    /// <param name="data">Данные для проверки</param>
    /// <returns>Результат валидации данных</returns>
    public ValidationResult ValidateData(object schema, object data)
    {
        var result = new ValidationResult();

        try
        {
            if (schema == null)
            {
                result.Errors.Add(new ValidationError
                {
                    Code = "SCHEMA_NULL",
                    Message = "Схема не может быть null",
                    PropertyPath = "$",
                    InvalidValue = null
                });
                return result;
            }

            // Базовая проверка данных
            if (data == null)
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Code = "DATA_NULL",
                    Message = "Данные равны null",
                    PropertyPath = "$",
                    Recommendation = "Проверьте корректность передаваемых данных"
                });
            }

            // TODO: Добавить полную валидацию данных по схеме
            // Это требует более сложной логики с использованием Json.Schema библиотеки
            
            result.IsValid = result.Errors.Count == 0;
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ValidationError
            {
                Code = "DATA_VALIDATION_ERROR",
                Message = $"Ошибка валидации данных: {ex.Message}",
                PropertyPath = "$",
                InvalidValue = data
            });
        }

        return result;
    }

    /// <summary>
    /// Валидировать базовую структуру схемы
    /// </summary>
    private static void ValidateBasicSchemaStructure(JsonElement root, ValidationResult result)
    {
        // Проверяем наличие обязательных полей
        if (!root.TryGetProperty("$schema", out _))
        {
            result.Warnings.Add(new ValidationWarning
            {
                Code = "MISSING_SCHEMA_VERSION",
                Message = "Отсутствует поле $schema",
                PropertyPath = "$.$schema",
                Recommendation = "Добавьте поле $schema со значением, например: \"https://json-schema.org/draft/2020-12/schema\""
            });
        }

        if (!root.TryGetProperty("type", out _))
        {
            result.Errors.Add(new ValidationError
            {
                Code = "MISSING_TYPE",
                Message = "Отсутствует обязательное поле type",
                PropertyPath = "$.type",
                InvalidValue = null
            });
        }
    }

    /// <summary>
    /// Валидировать общие поля схемы
    /// </summary>
    private static void ValidateCommonFields(JsonElement root, ValidationResult result)
    {
        // Проверяем поле title
        if (root.TryGetProperty("title", out var titleElement))
        {
            if (titleElement.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(titleElement.GetString()))
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Code = "INVALID_TITLE",
                    Message = "Поле title должно быть непустой строкой",
                    PropertyPath = "$.title",
                    Recommendation = "Укажите осмысленное название для схемы"
                });
            }
        }
        else
        {
            result.Warnings.Add(new ValidationWarning
            {
                Code = "MISSING_TITLE",
                Message = "Отсутствует поле title",
                PropertyPath = "$.title",
                Recommendation = "Добавьте описание схемы для лучшей документации"
            });
        }

        // Проверяем поле description
        if (root.TryGetProperty("description", out var descElement))
        {
            if (descElement.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(descElement.GetString()))
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Code = "INVALID_DESCRIPTION",
                    Message = "Поле description должно быть непустой строкой",
                    PropertyPath = "$.description",
                    Recommendation = "Укажите корректное описание схемы"
                });
            }
        }
    }

    /// <summary>
    /// Валидировать продвинутые правила
    /// </summary>
    private static void ValidateAdvancedRules(JsonElement root, ValidationResult result)
    {
        // Проверяем корректность типа
        if (root.TryGetProperty("type", out var typeElement))
        {
            var type = typeElement.GetString();
            if (!IsValidJsonSchemaType(type))
            {
                result.Errors.Add(new ValidationError
                {
                    Code = "INVALID_TYPE",
                    Message = $"Некорректный тип: {type}",
                    PropertyPath = "$.type",
                    InvalidValue = type
                });
            }
        }

        // Проверяем свойства для object типа
        if (root.TryGetProperty("type", out var objTypeElement) && 
            objTypeElement.ValueKind == JsonValueKind.String && 
            objTypeElement.GetString() == "object")
        {
            if (root.TryGetProperty("properties", out var propsElement))
            {
                if (propsElement.ValueKind != JsonValueKind.Object)
                {
                    result.Errors.Add(new ValidationError
                    {
                        Code = "INVALID_PROPERTIES",
                        Message = "Поле properties должно быть объектом",
                        PropertyPath = "$.properties",
                        InvalidValue = propsElement.GetRawText()
                    });
                }
            }
        }
    }

    /// <summary>
    /// Проверить корректность типа JSON Schema
    /// </summary>
    private static bool IsValidJsonSchemaType(string? type)
    {
        if (string.IsNullOrEmpty(type)) return false;
        
        var validTypes = new[] { "object", "array", "string", "number", "integer", "boolean", "null" };
        return validTypes.Contains(type);
    }
}