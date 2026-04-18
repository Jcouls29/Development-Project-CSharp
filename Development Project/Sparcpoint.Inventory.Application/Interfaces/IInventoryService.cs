namespace Sparcpoint.Inventory.Application.Interfaces
{
    public interface IInventoryService
    {
        Task AddInventory(List<int> productIds, decimal quantity);
        Task<decimal> GetInventory(int productId);
        Task DeleteTransaction(int transactionId);
    }
}