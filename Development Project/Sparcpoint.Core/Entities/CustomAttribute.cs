namespace Sparcpoint.Core.Entities;

public enum AttributeType
{
    Text,
    Number,
    Date,
    Boolean,
}

public record CustomAttribute
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    // Using string for quick prototype, but should be a more specific type
    public string? Value { get; init; }

    public AttributeType Type { get; init; } = AttributeType.Text;
}
