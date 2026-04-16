using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Interfaces
{
    public interface IInventoryService
    {
        Task<Models.InventoryTransactionModel> AddInventoryAsync(Models.InventoryTransactionRequest request);
        Task<Models.BulkInventoryResult> AddInventoryBulkAsync(Models.BulkInventoryRequest request);
        Task<Models.BulkInventoryResult> RemoveInventoryBulkAsync(Models.BulkInventoryRequest request);
        Task<Models.InventoryTransactionModel> RemoveInventoryAsync(Models.InventoryTransactionRequest request);
        Task<Models.InventoryCountModel> GetInventoryCountAsync(int productInstanceId);
        Task<Models.InventoryTransactionModel> UndoTransactionAsync(int transactionId);
        Task<Models.InventoryCountByMetadataModel> GetInventoryCountByMetadataAsync(System.Collections.Generic.Dictionary<string, string> attributes);
        Task<Models.PaginatedResult<Models.InventoryTransactionModel>> GetTransactionHistoryAsync(int productInstanceId, int page, int pageSize);
    }
}
