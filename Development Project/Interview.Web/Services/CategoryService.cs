using Interview.Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sparkpoint.Data;
using System.Collections.Generic;
using System.Linq;

namespace Interview.Web.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly InventoryContext _context;
        public CategoryService(InventoryContext context)
        {
            _context = context;
        }

        public Category CreateCategory(Category category)
        {
            throw new System.NotImplementedException();
        }

        public Category DeleteCategory(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            var categories = _context.Categories
                .Include(c => c.CategoryCategories_InstanceId)
                .ToList();
            return categories;
        }

        public Category GetCategoryById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Category UpdateCategory(Category category)
        {
            throw new System.NotImplementedException();
        }
    }
}
