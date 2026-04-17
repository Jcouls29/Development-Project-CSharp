namespace Interview.Application.UseCases.Query;

public static class SearchProductsConstraints
{
    public const int MinPage = 1;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;
    public const int MaxSearchTextLength = 256;
}
