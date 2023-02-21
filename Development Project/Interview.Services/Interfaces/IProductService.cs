using Interview.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Services.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Add a product
        /// </summary>
        /// <param name="product"></param>
        int Add(Product product);
    }
}
