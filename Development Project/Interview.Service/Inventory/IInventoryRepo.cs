using Interview.Service.Models;
using System.Collections.Generic;

namespace Interview.Service.Inventory
{
    public interface IInventoryRepo
    {
        void DeleteInventory(List<int> productId);
        List<InventoryTransaction> AddInventory(List<Product> products);
        int GetInventoryCount(ProductFilterParams parms);
    }
}
