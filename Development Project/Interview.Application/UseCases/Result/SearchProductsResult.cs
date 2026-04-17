using Interview.Application.UseCases.Query;

namespace Interview.Application.UseCases.Result;

public sealed record SearchProductsResult(
    List<ProductSummary> Items,
    int TotalCount,
    int Page,
    int PageSize);
