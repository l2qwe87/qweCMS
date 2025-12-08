namespace QweCMS.Core.Models;

/// <summary>
/// Результат выполнения операции
/// </summary>
/// <typeparam name="T">Тип возвращаемого значения</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Флаг успешного выполнения операции
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Возвращаемое значение (если операция успешна)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Список ошибок операции
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();

    /// <summary>
    /// Список предупреждений операции
    /// </summary>
    public List<ValidationWarning> Warnings { get; set; } = new();

    /// <summary>
    /// Сообщение об операции
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Создать успешный результат
    /// </summary>
    /// <param name="data">Данные</param>
    /// <param name="message">Сообщение</param>
    /// <returns>Успешный результат</returns>
    public static OperationResult<T> Success(T data, string? message = null)
    {
        return new OperationResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Создать неуспешный результат
    /// </summary>
    /// <param name="errors">Ошибки</param>
    /// <param name="message">Сообщение</param>
    /// <returns>Неуспешный результат</returns>
    public static OperationResult<T> Failure(IEnumerable<ValidationError> errors, string? message = null)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            Errors = errors.ToList(),
            Message = message
        };
    }

    /// <summary>
    /// Создать неуспешный результат с одной ошибкой
    /// </summary>
    /// <param name="error">Ошибка</param>
    /// <param name="message">Сообщение</param>
    /// <returns>Неуспешный результат</returns>
    public static OperationResult<T> Failure(ValidationError error, string? message = null)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            Errors = new List<ValidationError> { error },
            Message = message
        };
    }
}

/// <summary>
/// Результат выполнения операции без данных
/// </summary>
public class OperationResult : OperationResult<object>
{
    /// <summary>
    /// Создать успешный результат
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Успешный результат</returns>
    public static OperationResult SuccessResult(string? message = null)
    {
        return new OperationResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    /// <summary>
    /// Создать неуспешный результат
    /// </summary>
    /// <param name="errors">Ошибки</param>
    /// <param name="message">Сообщение</param>
    /// <returns>Неуспешный результат</returns>
    public static OperationResult FailureResult(IEnumerable<ValidationError> errors, string? message = null)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Errors = errors.ToList(),
            Message = message
        };
    }

    /// <summary>
    /// Создать неуспешный результат с одной ошибкой
    /// </summary>
    /// <param name="error">Ошибка</param>
    /// <param name="message">Сообщение</param>
    /// <returns>Неуспешный результат</returns>
    public static OperationResult FailureResult(ValidationError error, string? message = null)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Errors = new List<ValidationError> { error },
            Message = message
        };
    }
}