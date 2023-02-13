using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Abstract
{
    public interface IInventoryService
    {
        IEnumerable<InventoryTransaction> GetAllInventoryTransactions();
        Task<IEnumerable<InventoryTransaction>> GetAllInventoryTransactionsAsync();

        int GetInventoryForProduct(int productId);
        Task<int> GetInventoryForProductAsync(int productId);

        int AddInventoryTransaction(InventoryTransaction inventoryTransaction);
        Task<int> AddInventoryTransactionAsync(InventoryTransaction inventoryTransaction);

        int RollbackInventoryTransaction(int transactionId);
        Task<int> RollbackInventoryTransactionAsync(int transactionId);

        IEnumerable<Product> SearchInventory(ProductSearchScope searchBy, string keyword);
        Task<IEnumerable<Product>> SearchInventoryAsync(ProductSearchScope searchBy, string keyword);

        IEnumerable<Product> GetInventoryItemsByCount(int quantity, bool lessThan = false);
        Task<IEnumerable<Product>> GetInventoryItemsByCountAsync(int quantity, bool lessThan = false);
    }
}
