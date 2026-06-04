using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Repositories.Interfaces;

public interface IProductRepository
{
    Task<int> CreateAsync(CreateProductRequest request);
    Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request);
}