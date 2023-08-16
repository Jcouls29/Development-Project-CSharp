using Interview.Service.Models;
using System.Collections.Generic;

namespace Interview.Service.Products
{
    public interface IProductRepo
    {
        List<Product> RetreiveProducts(ProductFilterParams parms);
        List<Product> AddProducts(List<Product> products);
    }
}
