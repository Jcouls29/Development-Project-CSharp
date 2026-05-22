using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int productId);
        Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria);
        Task<int> CreateAsync(CreateProductRequest request);
    }
}
