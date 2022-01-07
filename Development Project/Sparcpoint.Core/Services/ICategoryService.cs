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
        Task AddAttributesToCategory(int categoryId, List<KeyValuePair<string, string>> attributes);
        Task AddCategoryToCategories(int categoryId, List<int> categories);
    }
}
