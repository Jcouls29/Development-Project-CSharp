using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Repositories.Interfaces;

public interface IInventoryRepository
{
    Task<int> AddAsync(AddInventoryRequest request);
    Task RemoveTransactionAsync(int transactionId);
    Task<decimal> GetCountAsync(InventoryCountRequest request);
}
