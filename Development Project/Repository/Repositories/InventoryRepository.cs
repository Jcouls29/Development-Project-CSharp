using Domain.Entitys;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        public IReadOnlyList<Inventory> AddProdcutsToInventory(List<Inventory> listofProductsToAdd)
        {
            return new List<Inventory>().AsReadOnly();
        }

        public int CountofInventoryByProductId(int productId)
        {
            return 1;
        }

        public void RemoveProdcutsToInventory(List<Inventory> listofProductsToAdd)
        {
           
        }

        public void SaveChangesAsync()
        {
           
        }
    }
}
