using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sparcpoint.Models;

namespace Sparcpoint.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> FindByIdAsync(int id);
    }
    public class CategoryRepository : ICategoryRepository
    {
        public CategoryRepository() { }

        public async Task<Category> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
