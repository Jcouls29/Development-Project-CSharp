using Interview.Web.Core.DomainModel;
using Interview.Web.Entities;
using System.Collections.Generic;

namespace Interview.Web.Core.Services
{
    public interface IProduct
    {
        ProductResponse AddProduct(ProdusctRequest products);
        IEnumerable<Products> SearchProduct();
    }
}
