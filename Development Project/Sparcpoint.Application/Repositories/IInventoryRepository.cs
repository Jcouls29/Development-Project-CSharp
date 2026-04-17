using System.Threading.Tasks;

namespace Sparcpoint.Application.Repositories
{
    public interface IInventoryRepository
    {
        // Record a transaction against a product instance. Returns generated TransactionId (int).
        Task<int> RecordTransactionAsync(int productInstanceId, decimal quantity, string transactionType, int? relatedTransactionId = null);

        // Mark a transaction as reversed/completed (best-effort; schema may vary).
        Task<bool> MarkAsReversedAsync(int transactionId);

        // Insert a compensating transaction for the specified transactionId and return the new TransactionId.
        Task<int> UndoTransactionAsync(int transactionId);
    }
}
