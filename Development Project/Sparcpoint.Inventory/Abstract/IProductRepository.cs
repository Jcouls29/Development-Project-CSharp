using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    // EVAL: Extends IInstanceRepository<Product> to inherit GetByIdAsync,
    // GetAllAsync, and AddAsync. Only product-specific operations are added here.
    // A new front-end (mobile, desktop) can depend on this interface without
    // knowing anything about SQL or Dapper underneath.
    public interface IProductRepository : IInstanceRepository<Product>
    {
        // EVAL: AddFromRequestAsync is the preferred add path over AddAsync(Product)
        // since it carries all the relational data (attributes, categories) in one call.
        // AddAsync(Product) on the base interface exists to satisfy the generic contract.
        Task<int> AddFromRequestAsync(CreateProductRequest request);
        Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request);
    }
}
