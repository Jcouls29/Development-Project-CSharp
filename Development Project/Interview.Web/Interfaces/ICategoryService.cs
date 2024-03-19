using Sparkpoint.Data;
using System.Collections.Generic;

namespace Interview.Web.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAllCategories();
        Category GetCategoryById(int id);
        Category CreateCategory(Category category);
        Category UpdateCategory(Category category);
        Category DeleteCategory(int id);
    }
}