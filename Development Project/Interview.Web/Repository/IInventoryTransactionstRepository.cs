using Interview.Web.Model;
using Sparcpoint;
using System.Threading.Tasks;

namespace Interview.Web.Repository
{
    public interface IInventoryTransactionsRepository : IAsyncCollection<InventoryTransactions>
    {
        Task<int> GetProductInventoryCount(InventoryTransactions inventoryTransactions);

    }
}
