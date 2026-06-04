using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Interview.Web.Services.Interfaces;

namespace Interview.Web.Services.Implementations;

public class ProductsService : IProductsService
{
    private readonly IProductRepository _productRepository;

    public ProductsService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // EVAL: Stubs throw NotImplementedException. Real logic driven by tests in Milestone 2 and 3.
    public Task<int> CreateAsync(CreateProductRequest request)
        => throw new System.NotImplementedException();

    public async Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request)
    {
        // EVAL: Passing null or empty request returns all products — no special branch needed
        // because the repository builds WHERE clauses only for non-null fields.
        return await _productRepository.SearchAsync(request);
    }
}