using Sparcpoint.Inventory.Application.Interfaces;

namespace Sparcpoint.Inventory.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;

        public InventoryService(IInventoryRepository repo)
        {
            _repo = repo;
        }

        public Task AddInventory(List<int> productIds, decimal quantity)
        {
            return _repo.AddInventoryAsync(productIds, quantity);
        }

        public Task<decimal> GetInventory(int productId)
        {
            return _repo.GetInventoryCountAsync(productId);
        }

        public Task DeleteTransaction(int transactionId)
        {
            return _repo.DeleteTransactionAsync(transactionId);
        }
    }
}