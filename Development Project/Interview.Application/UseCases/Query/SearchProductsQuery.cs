namespace Interview.Application.UseCases.Query;

public sealed record SearchProductsQuery
{
    public string? SearchText { get; init; }

    public List<int> CategoryIds { get; init; } = new();

    public List<AttributeFilterPair> AttributeFilters { get; init; } = new();

    public int Page { get; init; }

    public int PageSize { get; init; }
}
