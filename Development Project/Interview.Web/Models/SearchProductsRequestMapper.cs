using Interview.Application.UseCases.Query;

namespace Interview.Web.Models;

public static class SearchProductsRequestMapper
{
    public static SearchProductsQuery ToQuery(this SearchProductsRequest request)
    {
        var categories = request.CategoryIds ?? new List<int>();
        var attributes = (request.AttributeFilters ?? new List<SearchAttributeFilterRequest>())
            .Select(a => new AttributeFilterPair(a.Key, a.Value))
            .ToList();

        return new SearchProductsQuery
        {
            SearchText = request.SearchText,
            CategoryIds = categories,
            AttributeFilters = attributes,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
