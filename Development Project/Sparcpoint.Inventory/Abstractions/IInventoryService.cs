using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    public interface IInventoryService
    {
        Task<int> AddInventoryAsync(int productInstanceId, decimal quantity, CancellationToken cancellationToken = default);
        Task<int> RemoveInventoryAsync(int productInstanceId, decimal quantity, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<int>> AdjustInventoryBulkAsync(IReadOnlyList<(int ProductInstanceId, decimal Quantity)> adjustments, CancellationToken cancellationToken = default);
        Task<bool> UndoTransactionAsync(int transactionId, CancellationToken cancellationToken = default);
        Task<InventoryCount> GetCountAsync(int productInstanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<InventoryCount>> GetCountsByAttributeAsync(string key, string value, CancellationToken cancellationToken = default);
    }
}
