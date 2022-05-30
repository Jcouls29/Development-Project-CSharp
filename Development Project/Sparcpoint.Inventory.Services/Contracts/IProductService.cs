using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services.Contracts
{
    public interface IProductService
    {
        Task<int> AddProduct(Product product);
    }
}
