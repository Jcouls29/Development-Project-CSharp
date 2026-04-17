using Sparcpoint.Models;
using Sparcpoint.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Interfaces
{
    public interface IInventoryRepository
    {
        Task AddAsync(InventoryUpdateRequest request);

        Task RemoveAsync(InventoryUpdateRequest request);

        Task<decimal> GetCountAsync(int productInstanceId);

        Task<decimal> GetCountByAttributeAsync(string key, string value);

        Task UndoTransactionAsync(int transactionId);

        Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync(int productInstanceId);
    }
}
