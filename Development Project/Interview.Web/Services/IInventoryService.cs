using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Services
{
    public interface IInventoryService
    {
        Task<int> CreateProductAsync(CreateProductRequest request);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(ProductSearchRequest request);
        Task<decimal> GetInventoryCountAsync(int productInstanceId);
        Task<decimal> GetInventoryCountBySearchAsync(ProductSearchRequest request);
        Task<IEnumerable<InventoryTransactionDto>> GetInventoryTransactionsAsync(int productInstanceId);
        Task ProcessInventoryTransactionsAsync(List<InventoryTransactionRequest> requests);
        Task UndoInventoryTransactionAsync(int productInstanceId, int originalTransactionId);
    }
}
