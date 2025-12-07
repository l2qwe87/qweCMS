using QweCMS.Core.Entities;

namespace QweCMS.Core.Entities;

public class MixinEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object Schema { get; set; } = new { };
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}