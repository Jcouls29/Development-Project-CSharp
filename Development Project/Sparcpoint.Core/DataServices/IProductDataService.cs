using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    public interface IProductDataService
    {
        Task<List<Product>> GetProducts();
        Task<int> CreateProductAsync(Product newProduct);
        Task AddAttributeToProduct(int productId, KeyValuePair<string, string> attribute);
        Task AddProductToCategory(int categoryId, int productId);
        Task<List<Product>> SearchProducts(string keyword, List<string> searchBy, string orderBy, string orderDirection, int page, int pageCount);
    }
}
