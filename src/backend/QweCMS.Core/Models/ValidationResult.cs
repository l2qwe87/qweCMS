namespace QweCMS.Core.Models;

/// <summary>
/// Базовый класс для валидационных сообщений
/// </summary>
public abstract class ValidationMessage
{
    /// <summary>
    /// Код сообщения
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Путь к свойству где возникло сообщение
    /// </summary>
    public string? PropertyPath { get; set; }
}

/// <summary>
/// Ошибка валидации
/// </summary>
public class ValidationError : ValidationMessage
{
    /// <summary>
    /// Значение которое вызвало ошибку
    /// </summary>
    public object? InvalidValue { get; set; }
}

/// <summary>
/// Предупреждение валидации
/// </summary>
public class ValidationWarning : ValidationMessage
{
    /// <summary>
    /// Рекомендация по исправлению
    /// </summary>
    public string? Recommendation { get; set; }
}

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