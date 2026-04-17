using Interview.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public interface IInventoryService
    {
        Task<InventoryTransaction> AddInventoryAsync(Guid productId, int delta, string note);
        Task<int?> GetQuantityAsync(Guid productId);
        Task<int> GetQuantityByMetadataAsync(Dictionary<string, string> metadataCriteria);
        Task<bool> UndoLastInventoryAsync(Guid productId);
        Task<IEnumerable<InventoryTransaction>> AddInventoryBatchAsync(IEnumerable<InventoryTransaction> transactions);
        Task<bool> RemoveTransactionAsync(Guid transactionId);
    }
}
