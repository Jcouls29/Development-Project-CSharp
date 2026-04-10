using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories
{
    /// <summary>
    /// EVAL: Repository interface for inventory transaction operations.
    /// Supports adding/removing transactions and querying inventory counts.
    /// </summary>
    public interface IInventoryRepository
    {
        Task<InventoryTransaction> AddTransactionAsync(InventoryTransaction transaction);
        Task<bool> RemoveTransactionAsync(int transactionId);
        Task<decimal> GetInventoryCountAsync(int productInstanceId);
        Task<IEnumerable<InventoryCountSummary>> GetInventoryCountsByAttributeAsync(string key, string value);
    }

    /// <summary>
    /// Result type for inventory count queries grouped by product.
    /// </summary>
    public class InventoryCountSummary
    {
        public int ProductInstanceId { get; set; }
        public decimal Count { get; set; }
    }
}
