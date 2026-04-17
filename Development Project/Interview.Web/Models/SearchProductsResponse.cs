namespace Interview.Web.Models;

public sealed class SearchProductsResponse
{
    public required List<ProductSummaryResponse> Items { get; init; }

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }
}

public sealed class ProductSummaryResponse
{
    public int InstanceId { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }
}
