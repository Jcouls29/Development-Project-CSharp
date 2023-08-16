using Interview.Service.Models;
using System.Collections.Generic;

namespace Interview.Service.Inventory
{
    public interface IInventoryService
    {
        void DeleteInventory(List<int> productIds);
        List<InventoryTransaction> AddInventory(List<Product> products);
        int GetInventoryCount(ProductFilterParams parms);
    }
}
