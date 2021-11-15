using Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Interfaces
{
    public interface IInventoryRepository
    {

        IReadOnlyList<Inventory> AddProdcutsToInventory(List<Inventory> listofProductsToAdd);

        void RemoveProdcutsToInventory(List<Inventory> listofProductsToAdd);

        int CountofInventoryByProductId(int productId);

        void SaveChangesAsync();
    }
}
