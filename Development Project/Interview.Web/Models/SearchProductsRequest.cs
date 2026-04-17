using System.ComponentModel.DataAnnotations;
using Interview.Application.UseCases.Query;

namespace Interview.Web.Models;

public sealed class SearchProductsRequest
{
    [MaxLength(256)]
    public string? SearchText { get; init; }

    public List<int> CategoryIds { get; init; } = new();

    public List<SearchAttributeFilterRequest> AttributeFilters { get; init; } = new();

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, SearchProductsConstraints.MaxPageSize)]
    public int PageSize { get; init; } = 20;
}

public sealed class SearchAttributeFilterRequest
{
    [Required]
    [MaxLength(64)]
    public required string Key { get; init; }

    [Required]
    [MaxLength(512)]
    public required string Value { get; init; }
}
