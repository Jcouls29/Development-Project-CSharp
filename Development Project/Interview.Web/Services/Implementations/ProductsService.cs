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
        // EVAL: PreConditions from Sparcpoint.Core centralizes guard clause logic so every
        // service can validate inputs consistently without duplicating null/whitespace checks.
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
        // EVAL: Passing null or empty request returns all products — no special branch needed
        // because the repository builds WHERE clauses only for non-null fields.
        return await _productRepository.SearchAsync(request);
    }
}
