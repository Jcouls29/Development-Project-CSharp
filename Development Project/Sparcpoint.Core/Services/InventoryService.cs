using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<int> GetStockCountAsync(int productInstanceId)
        {
            return await _inventoryRepository.GetStockCountAsync(productInstanceId);
        }

        public async Task SaveTransactionAsync(List<InventoryUpdate> updates)
        {
            await _inventoryRepository.SaveTransactionAsync(updates);
        }

        public async Task UndoTransactionAsync(Guid transactionId)
        {
            await _inventoryRepository.UndoTransactionAsync(transactionId);
        }
    }
}
