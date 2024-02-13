namespace Sparcpoint.Core.Entities;

public record Product
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string ImageUris { get; init; }
    public required string ValidSkus { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime ModifiedDate { get; init; }
    public IEnumerable<CustomAttribute> Attributes { get; init; } = null!;
    public IEnumerable<Category> Categories { get; init; } = null!;
}
