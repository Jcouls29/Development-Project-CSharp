namespace Sparcpoint.Core.QueryObjects;

public record ProductSearchQuery
{
    public string? ProductName { get; init; }
    public string? ProductDescription { get; init; }
    public string? ProductImageUris { get; init; }
    public string? ProductValidSkus { get; init; }

    public DateTime? AfterDate { get; init; }
    public DateTime? BeforeDate { get; init; }

    public Dictionary<string, string>? Attributes { get; init; }
    public List<string>? Categories { get; init; }
}
