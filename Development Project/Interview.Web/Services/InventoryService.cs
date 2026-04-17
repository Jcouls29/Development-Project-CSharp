using Interview.Web.Models;
using Interview.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IProductRepository _productRepo;

        public InventoryService(IInventoryRepository inventoryRepo, IProductRepository productRepo)
        {
            _inventoryRepo = inventoryRepo;
            _productRepo = productRepo;
        }

        public Task<InventoryTransaction> AddInventoryAsync(Guid productId, int delta, string note)
        {
            return _inventoryRepo.AddInventoryAsync(productId, delta, note);
        }

        public Task<int?> GetQuantityAsync(Guid productId) => _inventoryRepo.GetQuantityAsync(productId);

        public async Task<int> GetQuantityByMetadataAsync(Dictionary<string, string> metadataCriteria)
        {
            var products = await _productRepo.SearchByMetadataAsync(metadataCriteria);
            return products.Sum(p => p.Quantity);
        }

        public Task<bool> UndoLastInventoryAsync(Guid productId)
        {
            return _inventoryRepo.UndoLastInventoryAsync(productId);
        }

        public Task<IEnumerable<InventoryTransaction>> AddInventoryBatchAsync(IEnumerable<InventoryTransaction> transactions)
        {
            return _inventoryRepo.AddInventoryBatchAsync(transactions);
        }

        public Task<bool> RemoveTransactionAsync(Guid transactionId)
        {
            return _inventoryRepo.RemoveTransactionAsync(transactionId);
        }
    }
}
