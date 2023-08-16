using Interview.Service.Models;
using System.Collections.Generic;

namespace Interview.Service.Inventory
{
    public interface IInventoryRepo
    {
        List<InventoryTransaction> AddInventory(List<InventoryTransaction> products);
        List<Product> GetProductInventory(ProductFilterParams parms);
    }
}
