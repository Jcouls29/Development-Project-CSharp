namespace Sparcpoint.Inventory.Application.Interfaces
{
    public interface IInventoryRepository
    {
        Task AddInventoryAsync(List<int> productIds, decimal quantity);
        Task<decimal> GetInventoryCountAsync(int productId);
        Task DeleteTransactionAsync(int transactionId);
    }
}