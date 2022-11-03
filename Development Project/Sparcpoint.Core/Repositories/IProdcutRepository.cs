using Sparcpoint.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Repositories
{
  public interface IProdcutRepository
    {
       Task<IEnumerable<Product>> GetProducts();
       Task<Product> CreateProduct(ProductForCreationDto product);
    }
}
