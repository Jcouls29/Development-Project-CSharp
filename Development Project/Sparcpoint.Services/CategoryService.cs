using Sparcpoint.DataRepository.Interfaces;
using Sparcpoint.Models.Request.Category;
using Sparcpoint.Models.Request.Product;
using Sparcpoint.Models.Response.Category;
using Sparcpoint.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        public async Task<GetCategoryResponse> GetAllCategories(GetCategoryRequest request)
        {
            var categories = await this._categoryRepository.GetAll();

            var categoryResponse = categories.Select(category =>
                new CategoryResponse
                {
                    CategoryId = category.InstanceId,
                    Name = category.Name,
                    Description = category.Description
                }).ToList();

            return new GetCategoryResponse
            {
                Categories = categoryResponse
            };
        }

        public async Task<List<CategoryResponse>> GetProductCategories(GetProductRequestById request)
        {
            var categories = await this._categoryRepository.GetProductCategories(request.ProductId);

            return categories.Select(category =>
                new CategoryResponse
                {
                    CategoryId = category.InstanceId,
                    Name = category.Name,
                    Description = category.Description
                }).ToList();
        }
    }
}