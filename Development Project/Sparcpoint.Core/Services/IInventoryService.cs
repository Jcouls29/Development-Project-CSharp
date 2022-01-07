using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public interface IInventoryService
    {
        Task<List<InventoryTransactions>> GetAllInventoryTransactions();
        Task<int> GetInventoryForProduct(int productId);
        Task<int> GetInventoryByMetadata(KeyValuePair<string, string> metadata);
        Task UpdateProductInventory(int productId, int newInventoryCount);
        Task RollbackInventoryUpdate(int transactionId);
    }
}
