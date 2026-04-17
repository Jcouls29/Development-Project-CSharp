namespace Interview.Application.UseCases.Command;

public sealed record CreateProductCommand
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required List<string> ProductImageUris { get; init; }
    public required List<string> ValidSkus { get; init; }
    public required List<CreateProductAttributeItem> Attributes { get; init; }
    public required List<int> CategoryIds { get; init; }
}

public sealed record CreateProductAttributeItem
{
    public required string Key { get; init; }
    public required string Value { get; init; }
}
