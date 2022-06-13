using Interview.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.IService
{
    public interface IProductService
    {
        Task<Product> AddProduct(Product product);
    }
}
