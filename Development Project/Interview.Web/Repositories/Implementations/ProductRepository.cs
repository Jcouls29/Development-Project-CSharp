using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Web.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly ISqlExecutor _sqlExecutor;

    public ProductRepository(ISqlExecutor sqlExecutor)
    {
        _sqlExecutor = sqlExecutor;
    }

    // EVAL: Stubs throw NotImplementedException so the compiler is satisfied while TDD
    // tests drive the real implementations in Milestone 2 and 3.
    public Task<int> CreateAsync(CreateProductRequest request)
        => throw new System.NotImplementedException();

    public Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request)
        => throw new System.NotImplementedException();
}