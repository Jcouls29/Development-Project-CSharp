using Sparcpoint.DataServices;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public class CategoryService: ICategoryService
    {
        ICategoryDataService _categoryDataService;

        public CategoryService(ICategoryDataService categoryDataService) {
            _categoryDataService = categoryDataService;
        }

        public async Task<List<Category>> GetCategories() {
            return await _categoryDataService.GetCategories();
        }

        public async Task CreateCategoryAsync(CreateCategoryRequest req) {

            //EVAL: eventually want to use a mapper here but start simple

            var category = new Category()
            {
                Name = req.Name,
                Description = req.Description,
                CreatedTimestamp = DateTime.UtcNow
            };

            var createdId = await _categoryDataService.CreateCategoryAsync(category);

            if(req.CategoryAttributes != null && req.CategoryAttributes.Count > 0)
            {
                //EVAL: for efficieny imporvement I'd want to write a method that accepts
                //multiple attributes at once to prevent too many DB connections
                foreach (var attr in req.CategoryAttributes)
                {
                    await _categoryDataService.AddAttributeToCategory(createdId, attr);
                }
            }

            if(req.CategoryIds != null && req.CategoryIds.Count > 0)
            {
                //EVAL: for efficieny imporvement I'd want to write a method that accepts
                //multiple attributes at once to prevent too many DB connections
                foreach (var cat in req.CategoryIds)
                {
                    await _categoryDataService.AddCategoryToCategory(createdId, cat);
                }
            }
        }


        public async Task AddAttributesToCategory(int categoryId, List<KeyValuePair<string, string>> attributes)
        {
            //EVAL: for efficieny imporvement I'd want to write a method that accepts
            //multiple attributes at once to prevent too many DB connections
            foreach (var attr in attributes)
            {
                await _categoryDataService.AddAttributeToCategory(categoryId, attr);
            }
        }

        public async Task AddCategoryToCategories(int categoryId, List<int> categories)
        {
            //EVAL: for efficieny imporvement I'd want to write a method that accepts
            //multiple attributes at once to prevent too many DB connections
            foreach (var cat in categories)
            {
                await _categoryDataService.AddCategoryToCategory(categoryId, cat);
            }
        }
    }
}
