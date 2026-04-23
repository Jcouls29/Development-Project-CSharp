using Interview.Web.Models;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public interface IInventoryService
    {
        // Adds inventory 
        Task AddInventoryAsync(AddInventoryRequest request);

        // Removes inventory 
        Task RemoveInventoryAsync(RemoveInventoryRequest request);

        // Retrieves current inventory 
        Task<InventoryCountResponse> GetInventoryCountAsync(int productId);
    }
}
