using Interview.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Repositories
{
    public interface IInventoryRepository
    {
        Task<InventoryTransaction> AddInventoryAsync(Guid productId, int delta, string note);
        Task<int?> GetQuantityAsync(Guid productId);
        Task<bool> UndoLastInventoryAsync(Guid productId);
        Task<IEnumerable<InventoryTransaction>> AddInventoryBatchAsync(IEnumerable<InventoryTransaction> transactions);
        Task<bool> RemoveTransactionAsync(Guid transactionId);
    }
}
