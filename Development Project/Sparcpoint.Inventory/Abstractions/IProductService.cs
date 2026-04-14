using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    public interface IProductService
    {
        Task<int> AddProductAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product?> GetProductAsync(int instanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default);
    }
}
