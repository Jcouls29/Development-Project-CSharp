using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;

namespace Sparcpoint.Core.Services
{
    // EVAL: Service Layer - Orchestrates business operations using Repository pattern
    // EVAL: Interface segregation - focused on inventory management operations
    public interface IInventoryService
    {
        // EVAL: Ledger pattern - Add inventory creates positive transaction
        Task<int> AddInventoryAsync(int productInstanceId, decimal quantity, string typeCategory = null);

        // EVAL: Bulk inventory operations support multiple products in a single request
        Task<IEnumerable<int>> AddInventoryAsync(IEnumerable<Models.InventoryAdjustment> adjustments);

        // EVAL: Ledger pattern - Remove inventory creates negative transaction
        Task<int> RemoveInventoryAsync(int productInstanceId, decimal quantity, string typeCategory = null);

        Task<IEnumerable<int>> RemoveInventoryAsync(IEnumerable<Models.InventoryAdjustment> adjustments);

        // EVAL: Undo capability - critical for ledger pattern to support reversals
        Task<bool> UndoTransactionAsync(int transactionId);

        // EVAL: Current inventory calculation via SUM of ledger transactions
        Task<decimal> GetCurrentInventoryAsync(int productInstanceId);

        // EVAL: Inventory reporting by metadata or category filters
        Task<decimal> GetInventoryCountByFilterAsync(string name = null, string description = null,
            IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null);
    }
}