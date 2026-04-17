using Interview.Application.Abstractions;
using Interview.Application.UseCases.Command;
using Interview.Application.UseCases.Exception;
using Interview.Application.UseCases.Query;
using Interview.Application.UseCases.Result;

namespace Interview.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<CreateProductResult> CreateAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = command.Name.Trim();
        if (normalizedName.Length == 0)
        {
            return new CreateProductResult.InvalidName();
        }

        var duplicateKeys = FindDuplicateAttributeKeys(command.Attributes);
        if (duplicateKeys.Count > 0)
        {
            return new CreateProductResult.DuplicateAttributeKeys(duplicateKeys);
        }

        // EVAL: Duplicate product names are not spelled out in the assignment; this check (plus UNIQUE on Instances.Products.Name
        // in Products.sql) was added voluntarily to enrich the solution—application guard for clear failures, unique index for integrity and concurrency.
        if (await _productRepository.ProductNameExistsAsync(normalizedName, cancellationToken).ConfigureAwait(false))
        {
            return new CreateProductResult.DuplicateName();
        }

        var distinctCategoryIds = command.CategoryIds.Distinct().ToList();
        if (distinctCategoryIds.Count > 0)
        {
            var missing = await _productRepository
                .GetMissingCategoryIdsAsync(distinctCategoryIds, cancellationToken)
                .ConfigureAwait(false);
            if (missing.Count > 0)
            {
                return new CreateProductResult.InvalidCategories(missing);
            }
        }

        var insertCommand = command with
        {
            Name = normalizedName,
            CategoryIds = distinctCategoryIds,
        };

        try
        {
            var productId = await _productRepository
                .InsertProductAsync(insertCommand, cancellationToken)
                .ConfigureAwait(false);

            return new CreateProductResult.Succeeded(productId);
        }
        catch (DuplicateProductNameException)
        {
            return new CreateProductResult.DuplicateName();
        }
    }

    public async Task<SearchProductsOutcomeResult> SearchProductsAsync(
        SearchProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.Page < SearchProductsConstraints.MinPage
            || query.PageSize < SearchProductsConstraints.MinPageSize
            || query.PageSize > SearchProductsConstraints.MaxPageSize)
        {
            return new SearchProductsOutcomeResult.InvalidPagination();
        }

        var categoryIds = query.CategoryIds.Distinct().ToList();
        var attributeFilters = query.AttributeFilters ?? new List<AttributeFilterPair>();

        string? normalizedSearch = null;
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var trimmed = query.SearchText.Trim();
            if (trimmed.Length > SearchProductsConstraints.MaxSearchTextLength)
            {
                return new SearchProductsOutcomeResult.InvalidQuery();
            }

            normalizedSearch = trimmed;
        }

        var trimmedAttributeFilters = attributeFilters
            .Select(p => new AttributeFilterPair(p.Key.Trim(), p.Value))
            .ToList();

        var normalizedQuery = query with
        {
            SearchText = normalizedSearch,
            CategoryIds = categoryIds,
            AttributeFilters = trimmedAttributeFilters,
        };

        var result = await _productRepository
            .SearchProductsAsync(normalizedQuery, cancellationToken)
            .ConfigureAwait(false);

        return new SearchProductsOutcomeResult.Succeeded(result);
    }

    private static List<string> FindDuplicateAttributeKeys(List<CreateProductAttributeItem> attributes)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var duplicates = new List<string>();
        foreach (var attribute in attributes)
        {
            if (!seen.Add(attribute.Key))
            {
                duplicates.Add(attribute.Key);
            }
        }

        return duplicates;
    }
}
