using Interview.Service.Models;
using System.Collections.Generic;

namespace Interview.Service.Inventory
{
    public interface IInventoryRepo
    {
        void DeleteInventory(List<int> productIds);
        List<InventoryTransaction> AddInventory(List<InventoryTransaction> products);
        int GetInventoryCount(ProductFilterParams parms);
    }
}
