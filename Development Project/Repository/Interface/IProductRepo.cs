using System;
using System.Collections.Generic;
using System.Text;

using Domain.Entities;

namespace Repository.Interface
{
   public interface IProductRepo
    {
        List<Product> GetAllProducts();
        void AddProduct(Product product);
        List<Product> SearchProduct(string keyword);
    }
}
