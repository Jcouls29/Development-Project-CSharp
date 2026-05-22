using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Repositories.Interfaces;
using Sparcpoint.Inventory.Services.Interfaces;

namespace Sparcpoint.Inventory.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            // EVAL: Validate parent categories exist
            if (request.ParentCategoryIds != null && request.ParentCategoryIds.Any())
            {
                var parents = await _categoryRepository.GetCategoriesByIdsAsync(request.ParentCategoryIds);
                var missingParents = request.ParentCategoryIds.Except(parents.Select(c => c.InstanceId)).ToList();
                
                if (missingParents.Any())
                {
                    throw new InvalidOperationException(
                        $"The following parent category IDs do not exist: {string.Join(", ", missingParents)}");
                }
            }

            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Attributes = request.Attributes ?? new Dictionary<string, string>(),
                ParentCategoryIds = request.ParentCategoryIds ?? new List<int>()
            };

            var created = await _categoryRepository.CreateCategoryAsync(category);
            
            _logger.LogInformation("Category {CategoryId} created successfully", created.InstanceId);
            
            return MapToCategoryResponse(created);
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int instanceId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(instanceId);
            return category != null ? MapToCategoryResponse(category) : null;
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(MapToCategoryResponse).ToList();
        }

        private CategoryResponse MapToCategoryResponse(Category category)
        {
            return new CategoryResponse
            {
                InstanceId = category.InstanceId,
                Name = category.Name,
                Description = category.Description,
                Attributes = category.Attributes,
                ParentCategoryIds = category.ParentCategoryIds,
                CreatedTimestamp = category.CreatedTimestamp
            };
        }
    }
}