using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory
{
    public interface IInventoryRepository
    {
        // EVAL: Add/Remove are separate methods (not a signed-quantity single method) to keep
        // intent explicit at the call site and allow validation of positive quantities.
        Task<int> AddAsync(int productInstanceId, decimal quantity, string typeCategory = null);
        Task AddBulkAsync(IEnumerable<(int ProductInstanceId, decimal Quantity, string TypeCategory)> transactions);
        Task RemoveAsync(int productInstanceId, decimal quantity, string typeCategory = null);
        Task RemoveBulkAsync(IEnumerable<(int ProductInstanceId, decimal Quantity, string TypeCategory)> transactions);

        Task DeleteTransactionAsync(int transactionId);

        Task<decimal> GetCountAsync(int productInstanceId);
        Task<decimal> GetCountByAttributesAsync(IDictionary<string, string> attributes);
    }
}
