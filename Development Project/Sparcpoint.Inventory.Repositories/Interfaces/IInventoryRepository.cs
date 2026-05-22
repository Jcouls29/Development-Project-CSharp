using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;

namespace Sparcpoint.Inventory.Repositories.Interfaces
{
    /// <summary>
    /// Repository for inventory transaction operations.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>
        /// Adds inventory transactions (bulk operation).
        /// EVAL: Supports requirement for adding inventory to multiple products at once.
        /// </summary>
        Task<List<InventoryTransaction>> AddInventoryAsync(List<InventoryTransaction> transactions);
        
        /// <summary>
        /// Removes a specific transaction (undo operation).
        /// EVAL: Supports requirement for ability to undo transactions.
        /// </summary>
        Task<bool> RemoveTransactionAsync(int transactionId);
        
        /// <summary>
        /// Gets inventory count based on criteria.
        /// EVAL: Supports requirement for retrieving counts by product or metadata.
        /// </summary>
        Task<List<InventoryCountResponse>> GetInventoryCountAsync(InventoryCountCriteria criteria);
        
        /// <summary>
        /// Gets transaction details by ID.
        /// </summary>
        Task<InventoryTransaction?> GetTransactionByIdAsync(int transactionId);
        
        /// <summary>
        /// Gets all transactions for a specific product.
        /// </summary>
        Task<List<InventoryTransaction>> GetTransactionsByProductIdAsync(int productInstanceId);
    }
}