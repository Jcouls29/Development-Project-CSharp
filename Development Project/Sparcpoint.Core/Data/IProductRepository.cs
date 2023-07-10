using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Data
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id, IDbTransaction transaction);
        Task<IEnumerable<Product>> GetAllAsync(IDbTransaction transaction);
        Task AddAsync(Product product, IDbTransaction transaction);
        Task UpdateAsync(Product product, IDbTransaction transaction);
        Task DeleteAsync(int id, IDbTransaction transaction);
    }
}
