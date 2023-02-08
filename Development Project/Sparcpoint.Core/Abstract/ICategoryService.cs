using Sparcpoint.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Abstracts
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategories();
        Task<List<Category>> GetCategoriesAsync();
        Task<int> CreateCategory(Category category);
        Task<int> CreateCategoryAsync(Category category);
        Task AddCategoryToCategory(int categoryId, CategoryOfCategory parentCategory);
        Task AddCategoryToCategoryAsync(int categoryId, CategoryOfCategory parentCategory);
        Task AddAttributeToCategory(int categoryId, List<CategoryAttribute> attributes);
        Task AddAttributeToCategoryAsync(int categoryId, List<CategoryAttribute> attributes);
        Task AddCategoryToCategories(int categoryId, List<CategoryOfCategory> categories);
        Task AddCategoryToCategoriesAsync(int categoryId, List<CategoryOfCategory> categories);
    }
}