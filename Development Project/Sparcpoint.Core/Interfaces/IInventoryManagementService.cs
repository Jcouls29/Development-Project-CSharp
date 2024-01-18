using Sparcpoint.Core.Models;
using System.Threading.Tasks;

namespace Sparcpoint.Core.Interfaces
{
    public interface IInventoryManagementService
    {
        Task AddToInventoryAsync(InventoryItem item);
        Task<int> GetInventoryCountAsync(int productId);
        Task RemoveFromInventoryAsync(InventoryItem item);
    }
}
