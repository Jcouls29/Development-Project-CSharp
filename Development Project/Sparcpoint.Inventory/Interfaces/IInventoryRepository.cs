using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Interfaces
{
    public interface IInventoryRepository
    {
        Task<int> AddTransactionAsync(Models.InventoryTransactionRequest request);
        Task<List<int>> AddBulkTransactionsAsync(List<Models.InventoryTransactionRequest> items);
        Task<Models.InventoryTransactionModel> GetTransactionByIdAsync(int transactionId);
        Task<Models.InventoryCountModel> GetTotalCountByProductAsync(int productInstanceId);
        Task CompleteTransactionAsync(int transactionId);
        Task<List<Models.InventoryCountModel>> GetCountByMetadataAsync(Dictionary<string, string> attributes);
        Task<Models.PaginatedResult<Models.InventoryTransactionModel>> GetTransactionsByProductAsync(int productInstanceId, int page, int pageSize);
    }
}
