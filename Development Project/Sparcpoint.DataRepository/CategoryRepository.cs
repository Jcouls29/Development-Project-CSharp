using Microsoft.EntityFrameworkCore;
using Sparcpoint.DataRepository.DbContext;
using Sparcpoint.DataRepository.Interfaces;
using Sparcpoint.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.DataRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SparcpointDbContext dbContext;

        public CategoryRepository(SparcpointDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Category>> GetAll()
        {
            return await this.dbContext.Categories.ToListAsync();
        }

        public async Task<List<Category>> GetProductCategories(int productId)
        {
            var productCategories = await this.dbContext.ProductCategories
                .Where(p => p.ProductInstanceId == productId)
                .ToListAsync();

            var categories = new List<Category>();

            if (!productCategories.Any())
            {
                return categories;
            }

            var allCategories = await this.dbContext.Categories.ToListAsync();

            foreach (var productCategory in productCategories)
            {
                var category = allCategories.FirstOrDefault(c => c.InstanceId == productCategory.CategoryInstanceId);
                categories.Add(category);
            }

            return categories;
        }
    }
}