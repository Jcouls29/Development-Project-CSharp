using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Data
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id, IDbTransaction transaction);
        Task<IEnumerable<Category>> GetAllAsync(IDbTransaction transaction);
        Task AddAsync(Category category, IDbTransaction transaction);
        Task UpdateAsync(Category category, IDbTransaction transaction);
        Task DeleteAsync(int id, IDbTransaction transaction);
    }
}
