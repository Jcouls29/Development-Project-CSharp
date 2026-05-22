using System;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Services
{
    public interface IInventoryService
    {
        Task<Inventory?> GetInventoryByProductId(Guid productId);
    }
}
