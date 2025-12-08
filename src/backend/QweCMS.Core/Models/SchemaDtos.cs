using QweCMS.Core.Entities;

namespace QweCMS.Core.Models;

/// <summary>
/// DTO для создания новой схемы
/// </summary>
public class CreateSchemaDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Collection { get; set; } = string.Empty;
    public List<MixinReference> Mixins { get; set; } = new();
    public object Schema { get; set; } = new { };
}

/// <summary>
/// DTO для обновления схемы
/// </summary>
public class UpdateSchemaDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Collection { get; set; } = string.Empty;
    public List<MixinReference> Mixins { get; set; } = new();
    public object Schema { get; set; } = new { };
}

/// <summary>
/// DTO для списка схем
/// </summary>
public class SchemaListDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Collection { get; set; } = string.Empty;
    public int MixinsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}