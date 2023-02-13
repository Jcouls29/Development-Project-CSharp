using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Abstract
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();
        Task<IEnumerable<Category>> GetCategoriesAsync();
        
        int CreateCategory(Category category);
        Task<int> CreateCategoryAsync(Category category);
        
        void AddCategoryToCategory(int categoryId, CategoryOfCategory parentCategory);
        Task AddCategoryToCategoryAsync(int categoryId, CategoryOfCategory parentCategory);
        
        void AddAttributeToCategory(int categoryId, IEnumerable<CategoryAttribute> attributes);
        Task AddAttributeToCategoryAsync(int categoryId, IEnumerable<CategoryAttribute> attributes);
        
        void AddCategoryToCategories(int categoryId, IEnumerable<CategoryOfCategory> categories);
        Task AddCategoryToCategoriesAsync(int categoryId, IEnumerable<CategoryOfCategory> categories);
    }
}
