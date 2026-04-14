using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    public interface IInventoryRepository
    {
        Task<int> RecordTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<int>> RecordTransactionsAsync(IReadOnlyList<InventoryTransaction> transactions, CancellationToken cancellationToken = default);
        Task<bool> RemoveTransactionAsync(int transactionId, CancellationToken cancellationToken = default);
        Task<InventoryCount> GetCountAsync(int productInstanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<InventoryCount>> GetCountsByAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    }
}
