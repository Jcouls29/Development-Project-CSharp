using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(CreateProductRequest req);
        Task<List<Product>> GetProducts();
        Task<List<Product>> SearchProducts(ProductSearchRequest req);
    }
}
