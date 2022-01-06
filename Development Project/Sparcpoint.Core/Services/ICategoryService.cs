using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategories();
        Task CreateCategoryAsync(CreateCategoryRequest req);
    }
}
