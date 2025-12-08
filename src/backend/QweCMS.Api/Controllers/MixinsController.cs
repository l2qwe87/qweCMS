using Microsoft.AspNetCore.Mvc;
using QweCMS.Core.Entities;
using QweCMS.Core.Services;
using QweCMS.Core.Models;

namespace QweCMS.Api.Controllers;

/// <summary>
/// Контроллер для управления миксинами в системе QweCMS
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MixinsController : ControllerBase
{
    private readonly IMixinService _mixinService;

    public MixinsController(IMixinService mixinService)
    {
        _mixinService = mixinService;
    }

    /// <summary>
    /// Получить все миксины
    /// </summary>
    /// <returns>Коллекция всех миксинов</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MixinEntity>>> GetAll()
    {
        var mixins = await _mixinService.GetAllAsync();
        return Ok(mixins);
    }

    /// <summary>
    /// Получить миксины с пагинацией и поиском
    /// </summary>
    /// <param name="parameters">Параметры поиска и пагинации</param>
    /// <returns>Результат с пагинацией</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<MixinEntity>>> GetPaged([FromQuery] SearchParameters parameters)
    {
        if (parameters.Page < 1)
            return BadRequest("Page must be greater than 0");
        
        if (parameters.PageSize < 1 || parameters.PageSize > 50)
            return BadRequest("Page size must be between 1 and 50");

        var result = await _mixinService.GetPagedAsync(parameters);
        return Ok(result);
    }

    /// <summary>
    /// Получить миксин по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <returns>Миксин или 404 если не найден</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<MixinEntity>> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("ID cannot be empty");

        var mixin = await _mixinService.GetByIdAsync(id);
        if (mixin == null)
        {
            return NotFound();
        }
        return Ok(mixin);
    }

    /// <summary>
    /// Создать новый миксин
    /// </summary>
    /// <param name="mixin">Данные миксина для создания</param>
    /// <returns>Созданный миксин с location header</returns>
    [HttpPost]
    public async Task<ActionResult<MixinEntity>> Create([FromBody] MixinEntity mixin)
    {
        var createdMixin = await _mixinService.CreateAsync(mixin);
        return CreatedAtAction(nameof(GetById), new { id = createdMixin.Id }, createdMixin);
    }

    /// <summary>
    /// Обновить существующий миксин
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <param name="mixin">Обновленные данные миксина</param>
    /// <returns>Обновленный миксин</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<MixinEntity>> Update(string id, [FromBody] MixinEntity mixin)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("ID cannot be empty");

        if (mixin == null)
            return BadRequest("Request body cannot be null");

        var updatedMixin = await _mixinService.UpdateAsync(id, mixin);
        return Ok(updatedMixin);
    }

    /// <summary>
    /// Удалить миксин по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <returns>204 No Content или 404 если не найден</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("ID cannot be empty");

        var result = await _mixinService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Валидировать JSON Schema
    /// </summary>
    /// <param name="schema">JSON Schema для валидации</param>
    /// <returns>Результат валидации с ошибками и предупреждениями</returns>
    [HttpPost("validate-schema")]
    public async Task<ActionResult<ValidationResult>> ValidateSchema([FromBody] object schema)
    {
        if (schema == null)
            return BadRequest("Schema cannot be null");

        var validationResult = await _mixinService.ValidateSchemaAsync(schema);
        return Ok(validationResult);
    }
}