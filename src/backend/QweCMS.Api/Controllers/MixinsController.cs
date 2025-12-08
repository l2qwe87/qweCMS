using Microsoft.AspNetCore.Mvc;
using QweCMS.Core.Entities;
using QweCMS.Core.Services;
using QweCMS.Core.Models;

namespace QweCMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MixinsController : ControllerBase
{
    private readonly IMixinService _mixinService;

    public MixinsController(IMixinService mixinService)
    {
        _mixinService = mixinService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MixinEntity>>> GetAll()
    {
        try
        {
            var mixins = await _mixinService.GetAllAsync();
            return Ok(mixins);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<MixinEntity>>> GetPaged([FromQuery] SearchParameters parameters)
    {
        try
        {
            if (parameters.Page < 1)
                return BadRequest("Page must be greater than 0");
            
            if (parameters.PageSize < 1 || parameters.PageSize > 50)
                return BadRequest("Page size must be between 1 and 50");

            var result = await _mixinService.GetPagedAsync(parameters);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MixinEntity>> GetById(string id)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<MixinEntity>> Create([FromBody] MixinEntity mixin)
    {
        try
        {
            var createdMixin = await _mixinService.CreateAsync(mixin);
            return CreatedAtAction(nameof(GetById), new { id = createdMixin.Id }, createdMixin);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MixinEntity>> Update(string id, [FromBody] MixinEntity mixin)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("ID cannot be empty");

            if (mixin == null)
                return BadRequest("Request body cannot be null");

            var updatedMixin = await _mixinService.UpdateAsync(id, mixin);
            return Ok(updatedMixin);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("validate-schema")]
    public async Task<ActionResult<bool>> ValidateSchema([FromBody] object schema)
    {
        try
        {
            if (schema == null)
                return BadRequest("Schema cannot be null");

            var isValid = await _mixinService.ValidateSchemaAsync(schema);
            return Ok(isValid);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}