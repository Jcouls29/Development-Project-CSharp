using Domain.Entities;
using SparcpointServices.Models;
using System;
using System.Collections.Generic;
using System.Text;
using entity = Domain.Entities;

namespace SparcpointServices.Interface
{
   public interface IProduct
    {
        List<ProductModel> GetAllProducts();
        void AddProduct(Product product);
        List<ProductModel> SearchProduct(string keyword);
    }
}
