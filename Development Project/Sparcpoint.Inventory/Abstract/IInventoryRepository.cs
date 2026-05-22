using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryTransaction>> CreateTransactionsAsync(IEnumerable<InventoryAdjustment> adjustments);
        Task DeleteTransactionAsync(int transactionId);
        Task<decimal> GetCountAsync(InventoryCountQuery query);
    }
}
