namespace Sparcpoint.Core.Entities;

public record Category
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public ICollection<CustomAttribute> Attributes { get; init; } = null!;
    public ICollection<Product> Products { get; init; } = null!;
}
