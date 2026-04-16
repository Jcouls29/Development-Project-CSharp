using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<int> AddTransactionAsync(InventoryTransactionRequest request);
        Task<decimal> GetStockAsync(int productId);
        Task<decimal> GetStockByMetadataAsync(string key, string value);
        Task<bool> RemoveTransactionAsync(int transactionId);
    }
}
