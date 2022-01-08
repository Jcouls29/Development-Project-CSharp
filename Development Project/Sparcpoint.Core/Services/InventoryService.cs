using Sparcpoint.DataServices;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public class InventoryService: IInventoryService
    {
        private IInventoryDataService _inventoryDataService;

        public InventoryService(IInventoryDataService inventoryDataService)
        {
            _inventoryDataService = inventoryDataService;
        }


        public async Task<List<InventoryTransactions>> GetAllInventoryTransactions()
        {
            return await _inventoryDataService.GetAllInventoryTransactions();
        }

        public async Task<int> GetInventoryForProduct(int productId)
        {
            return await _inventoryDataService.GetInventoryForProduct(productId);
        }

        public async Task UpdateProductInventory(int productId, int newInventoryCount)
        {
            var transaction = new InventoryTransactions();
            transaction.ProductInstanceId = productId;
            transaction.Quantity = newInventoryCount;
            transaction.StartedTimestamp = DateTime.UtcNow;
            transaction.CompletedTimestamp = DateTime.UtcNow;
            transaction.TypeCategory = string.Empty;

            await _inventoryDataService.AddNewInventoryTransaction(transaction);
        }

        public async Task RollbackInventoryUpdate(int transactionId)
        {
            await _inventoryDataService.RollbackInventoryUpdate(transactionId);
        }
    }
}
