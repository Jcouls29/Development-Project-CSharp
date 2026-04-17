using Interview.Application.UseCases.Command;
using Interview.Application.UseCases.Query;
using Interview.Application.UseCases.Result;

namespace Interview.Application.Abstractions;

public interface IProductRepository
{
    Task<bool> ProductNameExistsAsync(string name, CancellationToken cancellationToken = default);

    Task<List<int>> GetMissingCategoryIdsAsync(
        IReadOnlyCollection<int> categoryIds,
        CancellationToken cancellationToken = default);

    Task<int> InsertProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default);

    Task<SearchProductsResult> SearchProductsAsync(
        SearchProductsQuery query,
        CancellationToken cancellationToken = default);
}
