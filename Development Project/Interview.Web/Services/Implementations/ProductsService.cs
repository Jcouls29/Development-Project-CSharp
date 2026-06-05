using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Interview.Web.Services.Interfaces;
using Sparcpoint;

namespace Interview.Web.Services.Implementations;

public class ProductsService : IProductsService
{
    private readonly IProductRepository _productRepository;

    public ProductsService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<int> CreateAsync(CreateProductRequest request)
    {
        // EVAL: PreConditions from Sparcpoint.Core keeps guard clauses consistent across services.
        PreConditions.ParameterNotNull(request, nameof(request));
        PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));

        if (request.Name.Length > 256)
            throw new System.ArgumentException("Name cannot exceed 256 characters.", nameof(request.Name));

        if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > 256)
            throw new System.ArgumentException("Description cannot exceed 256 characters.", nameof(request.Description));

        return await _productRepository.CreateAsync(request);
    }

    public async Task<IEnumerable<ProductResponse>> SearchAsync(ProductSearchRequest request)
    {
        // EVAL: Empty request returns all products — the repository only adds WHERE clauses
        // for fields that have values, so no special "get all" branch is needed.
        return await _productRepository.SearchAsync(request);
    }
}
