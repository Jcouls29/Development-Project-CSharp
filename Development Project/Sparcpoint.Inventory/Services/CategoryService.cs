using Sparcpoint;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<CategoryDetailModel> CreateCategoryAsync(CreateCategoryRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));
            PreConditions.StringNotNullOrWhitespace(request.Description, nameof(request.Description));

            if (request.Name.Length > 64)
                throw new ArgumentException("Name must be 64 characters or less.", nameof(request.Name));

            if (request.Description.Length > 256)
                throw new ArgumentException("Description must be 256 characters or less.", nameof(request.Description));

            var instanceId = await _categoryRepository.CreateAsync(request);
            return await _categoryRepository.GetByIdAsync(instanceId);
        }

        public Task<CategoryDetailModel> GetCategoryAsync(int instanceId)
        {
            if (instanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(instanceId));

            return _categoryRepository.GetByIdAsync(instanceId);
        }

        public Task<PaginatedResult<CategoryModel>> GetCategoriesAsync(int page, int pageSize)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return _categoryRepository.GetAllAsync(page, pageSize);
        }

        public Task AddAttributeAsync(int instanceId, string key, string value)
        {
            if (instanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(instanceId));

            PreConditions.StringNotNullOrWhitespace(key, nameof(key));
            PreConditions.StringNotNullOrWhitespace(value, nameof(value));

            return _categoryRepository.AddAttributeAsync(instanceId, key, value);
        }

        public Task RemoveAttributeAsync(int instanceId, string key)
        {
            if (instanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(instanceId));

            PreConditions.StringNotNullOrWhitespace(key, nameof(key));

            return _categoryRepository.RemoveAttributeAsync(instanceId, key);
        }

        public Task AddParentCategoryAsync(int instanceId, int parentCategoryId)
        {
            if (instanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(instanceId));

            if (parentCategoryId <= 0)
                throw new ArgumentOutOfRangeException(nameof(parentCategoryId));

            return _categoryRepository.AddParentCategoryAsync(instanceId, parentCategoryId);
        }

        public Task RemoveParentCategoryAsync(int instanceId, int parentCategoryId)
        {
            if (instanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(instanceId));

            if (parentCategoryId <= 0)
                throw new ArgumentOutOfRangeException(nameof(parentCategoryId));

            return _categoryRepository.RemoveParentCategoryAsync(instanceId, parentCategoryId);
        }
    }
}
