using Microsoft.EntityFrameworkCore;
using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Context;
using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Entities;
using Sparcpoint.Infrastructure.Services.Interfaces;

namespace Sparcpoint.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly SparcpointBaseContext _context;

        public CategoryService(SparcpointBaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            try
            {
                // EVAL: Validate the category data before proceeding. I would use FluentValidation for this
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // EVAL: Log the exception.
                // Would build a middleware to handle this that will remove having to use a try/catch block
                // or build a custom exception handler nuget package which would use the try/catch block.

                // Logger.Log(ex)
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // EVAL: Add admin role check here to ensure only admins can delete categories.
            var category = await _context.Categories.FindAsync(id);

            if (category is null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<List<Category>> SearchCategoriesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await _context.Categories.Where(c => c.Name.Contains(name)).ToListAsync();
        }

        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.InstanceId);
            if (category is null)
            {
                return null;
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();

            return existingCategory;
        }
    }
}