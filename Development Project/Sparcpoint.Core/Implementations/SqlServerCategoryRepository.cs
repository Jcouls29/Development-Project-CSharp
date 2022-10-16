using Sparcpoint.Abstract;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
    public class SqlServerCategoryRepository : ICategoryRepository
    {
        public Task<bool> AddCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
