using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IProductRepository
    {
        Task<int> AddProductAsync(Product product);
    }
}
