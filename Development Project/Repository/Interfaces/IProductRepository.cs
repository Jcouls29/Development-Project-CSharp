using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository.Interfaces
{
    public interface IProductRepository
    {

        IEnumerable<Product> GetProudcts();
        Product GetProductById(int productId);
        Product InsertProduct(Product product);
        void DeleteProduct(int productId);
        void UpdateCustomer(Product product);
        void  SaveChangesAsync();
    }
}
