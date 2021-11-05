using Interview.Web.Model;
using Sparcpoint;
using System.Threading.Tasks;

namespace Interview.Web.Repository
{
    public interface IInventoryRepository : IAsyncCollection<InventoryTransactions>
    {
        Task<int> GetProductInventoryCount(InventoryTransactions inventoryTransactions);

    }
}
