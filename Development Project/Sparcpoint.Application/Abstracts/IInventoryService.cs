using Sparcpoint.Domain.Instance.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Abstracts
{
    public interface IInventoryService
    {
        Task<List<InventoryTransaction>> GetAllInventoryTransactions();
        Task<List<InventoryTransaction>> GetAllInventoryTransactionsAsync();
        Task<int> GetInventoryForProduct(int productId);
        Task<int> GetInventoryForProductAsync(int productId);
        Task<int> AddInventoryTransaction(InventoryTransaction transaction);
        Task<int> AddInventoryTransactionAsync(InventoryTransaction transaction);
        Task<int> RollbackInventoryTransaction(int transactionId);
        Task<int> RollbackInventoryTransactionAsync(int transactionId);
        Task<List<Product>> SearchInventoryAsync(string keyword);


    }
}
