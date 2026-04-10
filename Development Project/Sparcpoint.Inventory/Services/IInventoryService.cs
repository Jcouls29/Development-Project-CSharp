using Sparcpoint.Inventory.Repositories;

namespace Sparcpoint.Inventory.Services
{
    /// <summary>
    /// EVAL: Service interface for inventory management operations.
    /// Encapsulates the business rules for inventory transactions.
    /// </summary>
    public interface IInventoryService
    {
        Task<Models.InventoryTransaction> AddToInventoryAsync(int productInstanceId, decimal quantity, string? typeCategory = null);
        Task<Models.InventoryTransaction> RemoveFromInventoryAsync(int productInstanceId, decimal quantity, string? typeCategory = null);
        Task<bool> UndoTransactionAsync(int transactionId);
        Task<decimal> GetInventoryCountAsync(int productInstanceId);
        Task<IEnumerable<InventoryCountSummary>> GetInventoryCountsByAttributeAsync(string key, string value);
    }
}
