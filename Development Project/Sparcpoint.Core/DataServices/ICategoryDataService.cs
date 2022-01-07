using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    public interface ICategoryDataService
    {
        Task<List<Category>> GetCategories();
        Task<int> CreateCategoryAsync(Category newProduct);
        Task AddCategoryToCategory(int categoryId, int parentCategoryId);
        Task AddAttributeToCategory(int categoryId, KeyValuePair<string, string> attribute);
    }
}
