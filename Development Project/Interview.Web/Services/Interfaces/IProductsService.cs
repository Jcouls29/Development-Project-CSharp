using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Services.Interfaces;

public interface IProductsService
{
    Task<int> CreateAsync(CreateProductRequest request);
    Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request);
}