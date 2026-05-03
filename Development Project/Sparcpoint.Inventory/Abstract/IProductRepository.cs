using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    public interface IProductRepository
    {
        Task<int> AddAsync(AddProductRequest request);
        Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request);
    }
}
