using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QweCMS.Api.Filters;

/// <summary>
/// Глобальный фильтр для обработки исключений в API
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Обрабатывает исключения, возникающие в контроллерах
    /// </summary>
    /// <param name="context">Контекст исключения</param>
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Произошла ошибка при обработке запроса: {RequestPath}", context.HttpContext.Request.Path);

        var response = new
        {
            error = "Internal server error",
            message = context.Exception.Message,
            timestamp = DateTime.UtcNow
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = 500
        };
        
        context.ExceptionHandled = true;
    }
}