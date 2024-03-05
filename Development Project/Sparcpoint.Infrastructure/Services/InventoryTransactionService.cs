using Microsoft.EntityFrameworkCore;
using Sparcpoint.Core.Entities;
using Sparcpoint.Core.Interfaces;
using Sparcpoint.Core.QueryObjects;
using Sparcpoint.Infrastructure.Data;

namespace Sparcpoint.Infrastructure.Services;

public class InventoryTransactionService(ApplicationDbContext dbContext) : IInventoryTransactionService
{
    public IAsyncEnumerable<InventoryTransaction> GetTransactionsAsync(long productId)
    {
        return dbContext.InventoryTransactions
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<InventoryTransaction> GetTransactionsAsync(List<long> productIds)
    {
        // filter isn't working. Need to fix this. Could be a sqlite thing
        return dbContext.InventoryTransactions
            .Where(t => productIds.Contains(t.ProductId))
            .AsAsyncEnumerable();
    }

    public Task<InventoryTransaction?> GetTransactionByIdAsync(int id)
    {
        return dbContext.InventoryTransactions
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<InventoryTransaction>> AddProductTransactionAsync(List<(long productId, decimal quantity)> products)
    {
        var productTransactions = products.Select(p => new InventoryTransaction
        {
            ProductId = p.productId,
            Quantity = p.quantity,
            StartDate = DateTime.UtcNow,
        }).ToList();

        dbContext.InventoryTransactions.AddRange(productTransactions);

        await dbContext.SaveChangesAsync();

        return productTransactions;
    }

    public Task<decimal> GetTotalProductQuantityAsync(long productId)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetTotalProductQuantityAsync(ProductSearchQuery searchQuery)
    {
        throw new NotImplementedException();
    }
}
