using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

/// <summary>
/// Интерфейс сервиса компоновки схем из миксинов
/// </summary>
public interface ISchemaCompositionService
{
    /// <summary>
    /// Компоновать схему из миксинов
    /// </summary>
    /// <param name="schema">Схема с миксинами</param>
    /// <returns>Результат компоновки с скомпонованной схемой</returns>
    Task<OperationResult<object>> ComposeSchemaAsync(SchemaEntity schema);

    /// <summary>
    /// Валидировать пути миксинов в схеме
    /// </summary>
    /// <param name="schema">Схема для валидации</param>
    /// <returns>Результат валидации</returns>
    Task<ValidationResult> ValidateMixinPathsAsync(SchemaEntity schema);

    /// <summary>
    /// Проверить на циклические зависимости между миксинами
    /// </summary>
    /// <param name="schema">Схема для проверки</param>
    /// <returns>Результат проверки</returns>
    Task<ValidationResult> CheckCircularDependenciesAsync(SchemaEntity schema);

    /// <summary>
    /// Очистить кеш скомпонованных схем
    /// </summary>
    void ClearCache();
}