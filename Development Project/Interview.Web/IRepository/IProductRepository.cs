using Interview.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.IRepository
{
    public interface IProductRepository
    {
        Task<Product> Add(Product product);

    }
}
