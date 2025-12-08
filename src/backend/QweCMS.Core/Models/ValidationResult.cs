namespace QweCMS.Core.Models;

/// <summary>
/// Результат валидации JSON Schema
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Флаг успешной валидации
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Список ошибок валидации
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();

    /// <summary>
    /// Список предупреждений валидации
    /// </summary>
    public List<ValidationWarning> Warnings { get; set; } = new();
}

/// <summary>
/// Ошибка валидации
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Путь к свойству где возникла ошибка
    /// </summary>
    public string? PropertyPath { get; set; }

    /// <summary>
    /// Значение которое вызвало ошибку
    /// </summary>
    public object? InvalidValue { get; set; }
}

/// <summary>
/// Предупреждение валидации
/// </summary>
public class ValidationWarning
{
    /// <summary>
    /// Код предупреждения
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение предупреждения
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Путь к свойству где возникло предупреждение
    /// </summary>
    public string? PropertyPath { get; set; }

    /// <summary>
    /// Рекомендация по исправлению
    /// </summary>
    public string? Recommendation { get; set; }
}