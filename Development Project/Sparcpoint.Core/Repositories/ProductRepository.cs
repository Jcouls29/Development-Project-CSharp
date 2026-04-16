using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Repositories
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
    }
    public class ProductRepository : IProductRepository
    {
        public ProductRepository() { }

        public async Task AddAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
