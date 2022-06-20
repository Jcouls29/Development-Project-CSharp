using Interview.Web.Models;
using System.Collections.Generic;

namespace Interview.Web.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
        void AddProduct(Product product);
        int UpdateInventory(Transaction transaction);
    }
}
