using Interview.Web.Models;
using System.Collections.Generic;

namespace Interview.Web.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        List<Product> GetInventory(Product product);
        List<Product> GetInventory(string productname, string description, string validSkus, string categoryName);
        Product Add(Product product);
        void Remove(int id);
        int GetActiveProductCount();

    }
}
