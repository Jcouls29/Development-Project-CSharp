using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.DataEntities.Models;

namespace Interview.Services
{
    public interface IInventoryTransactionService
    {
        Task<int> RecordInventoryTransactionAsync(InventoryRequest request);
        Task RecordInventoryTransactionsAsync(IEnumerable<InventoryRequest> requests);
        Task UndoInventoryTransactionAsync(int transactionId);
        Task<decimal> GetProductStockAsync(int productId);
        Task<decimal> GetStockByMetadataAsync(string key, string value);
    }
}
