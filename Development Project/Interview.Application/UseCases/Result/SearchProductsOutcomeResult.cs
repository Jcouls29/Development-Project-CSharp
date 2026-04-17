namespace Interview.Application.UseCases.Result;

public abstract record SearchProductsOutcomeResult
{
    public sealed record Succeeded(SearchProductsResult Result) : SearchProductsOutcomeResult;

    public sealed record InvalidPagination : SearchProductsOutcomeResult;

    public sealed record InvalidQuery : SearchProductsOutcomeResult;
}
