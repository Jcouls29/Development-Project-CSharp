using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    public interface IInventoryDataService
    {
        Task<List<InventoryTransactions>> GetAllInventoryTransactions();

        Task<int> GetInventoryForProduct(int productId);

        Task<int> GetInventoryByMetadata(KeyValuePair<string, string> metadata);

        Task AddNewInventoryTransaction(InventoryTransactions transaction);

        Task RollbackInventoryUpdate(int transactionId);
    }
}
