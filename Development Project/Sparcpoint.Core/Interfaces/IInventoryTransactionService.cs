using Sparcpoint.Core.Entities;
using Sparcpoint.Core.QueryObjects;

namespace Sparcpoint.Core.Interfaces;

public interface IInventoryTransactionService
{
    public IAsyncEnumerable<InventoryTransaction> GetTransactionsAsync(long productId);
    public IAsyncEnumerable<InventoryTransaction> GetTransactionsAsync(List<long> productIds);
    public Task<InventoryTransaction?> GetTransactionByIdAsync(int id);

    public Task<List<InventoryTransaction>> AddProductTransactionAsync(List<(long productId, decimal quantity)> products);

    public Task<decimal> GetTotalProductQuantityAsync(long productId);
    public Task<decimal> GetTotalProductQuantityAsync(ProductSearchQuery searchQuery);
}
