using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories.Contracts
{
    public interface IProductRepository
    {
        Task<int> AddProduct(Product product);
    }
}
