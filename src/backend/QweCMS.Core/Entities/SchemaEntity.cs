using QweCMS.Core.Entities;

namespace QweCMS.Core.Entities;

public class SchemaEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Collection { get; set; } = string.Empty;
    public List<MixinReference> Mixins { get; set; } = new List<MixinReference>();
    public object Schema { get; set; } = new { };
}

public class MixinReference
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}