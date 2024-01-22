using Sparcpoint.Models.Requests;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IInventoryService
    {
        Task UpdateProductInventoryAsync(UpdateInventoryRequest request);
        Task GetProductInventoryAsync();
    }
}