using Sparcpoint.Abstract;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
    public class SqlServerProductRepository : IProductRepository
    {
        public Task<bool> AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
