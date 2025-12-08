using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

/// <summary>
/// Интерфейс сервиса валидации JSON Schema
/// </summary>
public interface IJsonSchemaValidationService
{
    /// <summary>
    /// Валидировать JSON Schema
    /// </summary>
    /// <param name="schema">Схема для валидации</param>
    /// <returns>Результат валидации с ошибками и предупреждениями</returns>
    ValidationResult ValidateSchema(object schema);

    /// <summary>
    /// Валидировать JSON Schema по строке
    /// </summary>
    /// <param name="schemaJson">Схема в формате JSON строки</param>
    /// <returns>Результат валидации с ошибками и предупреждениями</returns>
    ValidationResult ValidateSchemaJson(string schemaJson);

    /// <summary>
    /// Валидировать данные по схеме
    /// </summary>
    /// <param name="schema">Схема для валидации</param>
    /// <param name="data">Данные для проверки</param>
    /// <returns>Результат валидации данных</returns>
    ValidationResult ValidateData(object schema, object data);
}