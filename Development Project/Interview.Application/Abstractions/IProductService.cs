using Interview.Application.UseCases.Command;
using Interview.Application.UseCases.Query;
using Interview.Application.UseCases.Result;

namespace Interview.Application.Abstractions;

public interface IProductService
{
    Task<CreateProductResult> CreateAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken = default);

    Task<SearchProductsOutcomeResult> SearchProductsAsync(
        SearchProductsQuery query,
        CancellationToken cancellationToken = default);
}
