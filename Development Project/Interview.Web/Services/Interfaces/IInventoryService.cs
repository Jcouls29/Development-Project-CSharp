using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Services.Interfaces;

public interface IInventoryService
{
    Task<int> AddAsync(AddInventoryRequest request);
    Task RemoveTransactionAsync(int transactionId);
    Task<decimal> GetCountAsync(InventoryCountRequest request);
}
