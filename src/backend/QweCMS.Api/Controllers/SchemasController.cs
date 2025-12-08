using Microsoft.AspNetCore.Mvc;
using QweCMS.Core.Entities;
using QweCMS.Core.Services;
using QweCMS.Core.Models;

namespace QweCMS.Api.Controllers;

/// <summary>
/// Контроллер для управления схемами в системе QweCMS
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SchemasController : ControllerBase
{
    private readonly ISchemaService _schemaService;

    public SchemasController(ISchemaService schemaService)
    {
        _schemaService = schemaService;
    }

    /// <summary>
    /// Получить все схемы
    /// </summary>
    /// <returns>Коллекция всех схем</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SchemaEntity>>> GetAll()
    {
        var schemas = await _schemaService.GetAllAsync();
        return Ok(schemas);
    }

    /// <summary>
    /// Получить схемы с пагинацией и поиском
    /// </summary>
    /// <param name="parameters">Параметры поиска и пагинации</param>
    /// <returns>Результат с пагинацией</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<SchemaEntity>>> GetPaged([FromQuery] SearchParameters parameters)
    {
        if (parameters.Page < 1)
            return BadRequest("Page must be greater than 0");

        if (parameters.PageSize < 1 || parameters.PageSize > 50)
            return BadRequest("Page size must be between 1 and 50");

        var result = await _schemaService.GetPagedAsync(parameters);
        return Ok(result);
    }

    /// <summary>
    /// Получить схему по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>Схема или 404 если не найдена</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SchemaEntity>> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("ID cannot be empty");

        var schema = await _schemaService.GetByIdAsync(id);
        if (schema == null)
        {
            return NotFound();
        }
        return Ok(schema);
    }

    /// <summary>
    /// Получить скомпонованную схему с миксинами
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>Скомпонованная схема</returns>
    [HttpGet("{id}/composed")]
    public async Task<ActionResult<object>> GetComposed(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("ID cannot be empty");

        var result = await _schemaService.GetComposedSchemaAsync(id);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Создать новую схему
    /// </summary>
    /// <param name="schema">Данные схемы для создания</param>
    /// <returns>Результат операции создания</returns>
    [HttpPost]
    public async Task<ActionResult<OperationResult<SchemaEntity>>> Create([FromBody] SchemaEntity schema)
    {
        var result = await _schemaService.CreateAsync(schema);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Обновить существующую схему
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <param name="schema">Обновленные данные схемы</param>
    /// <returns>Результат операции обновления</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<OperationResult<SchemaEntity>>> Update(string id, [FromBody] SchemaEntity schema)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(OperationResult<SchemaEntity>.Failure(
                new ValidationError { Code = "INVALID_ID", Message = "ID cannot be empty" },
                "Invalid ID"));

        if (schema == null)
            return BadRequest(OperationResult<SchemaEntity>.Failure(
                new ValidationError { Code = "NULL_BODY", Message = "Request body cannot be null" },
                "Invalid request body"));

        var result = await _schemaService.UpdateAsync(id, schema);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Удалить схему по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>Результат операции удаления</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<OperationResult<bool>>> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(OperationResult<bool>.Failure(
                new ValidationError { Code = "INVALID_ID", Message = "ID cannot be empty" },
                "Invalid ID"));

        var result = await _schemaService.DeleteAsync(id);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        if (result.Errors.Any(e => e.Code == "NOT_FOUND"))
        {
            return NotFound(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Валидировать связи с миксинами
    /// </summary>
    /// <param name="schema">Схема для валидации</param>
    /// <returns>Результат валидации</returns>
    [HttpPost("validate-mixins")]
    public async Task<ActionResult<ValidationResult>> ValidateMixins([FromBody] SchemaEntity schema)
    {
        if (schema == null)
            return BadRequest("Schema cannot be null");

        var validationResult = await _schemaService.ValidateMixinsAsync(schema);
        return Ok(validationResult);
    }
}