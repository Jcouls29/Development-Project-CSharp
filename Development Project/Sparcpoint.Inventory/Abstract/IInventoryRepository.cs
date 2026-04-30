using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Requests;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    // EVAL: Inventory operations are intentionally separate from IProductRepository.
    // Products and inventory have different lifecycles — keeping them decoupled
    // means inventory logic can be swapped (e.g. to an event-sourced store)
    // without touching product management code.
    public interface IInventoryRepository
    {
        Task<int> AddAsync(AddInventoryRequest request);
        Task<int> RemoveAsync(RemoveInventoryRequest request);

        // EVAL: RemoveTransactionAsync is the "undo" mechanic from the spec.
        // Deleting a specific transaction row effectively reverses it since
        // inventory count is always SUM(Quantity) over all transactions.
        Task RemoveTransactionAsync(int transactionId);

        Task<decimal> GetInventoryCountAsync(int productInstanceId);
    }
}
