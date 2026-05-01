using Sparcpoint.Inventory.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    public interface IInventoryRepository
    {
        Task<int> AddAsync(InventoryAdjustmentRequest request);
        Task<IEnumerable<int>> AddBatchAsync(IEnumerable<InventoryAdjustmentRequest> requests);
        Task<int> RemoveAsync(InventoryAdjustmentRequest request);
        Task<IEnumerable<int>> RemoveBatchAsync(IEnumerable<InventoryAdjustmentRequest> requests);
        Task<decimal> GetCountAsync(int productInstanceId);
        Task<decimal> GetCountByMetadataAsync(InventoryCountByMetadataRequest request);
        Task RemoveTransactionAsync(int transactionId);
    }
}
