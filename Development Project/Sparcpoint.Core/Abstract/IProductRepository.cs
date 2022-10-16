using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IProductRepository
    {
        Task<bool> AddProductAsync(Product product);
    }
}
