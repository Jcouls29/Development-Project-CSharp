using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories;

namespace Sparcpoint.Inventory.Services
{
    /// <summary>
    /// EVAL: Implements category business logic with input validation.
    /// Validates string lengths against database column constraints.
    /// Validates parent category IDs exist before creating hierarchical links.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _CategoryRepository;

        private const int MaxNameLength = 64;
        private const int MaxDescriptionLength = 256;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _CategoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            PreConditions.ParameterNotNull(category, nameof(category));
            PreConditions.StringNotNullOrWhitespace(category.Name, nameof(category.Name));
            PreConditions.StringNotNullOrWhitespace(category.Description, nameof(category.Description));

            // EVAL: Validate against DB column constraints — Categories.Name is VARCHAR(64)
            if (category.Name.Length > MaxNameLength)
                throw new ArgumentException($"Category name cannot exceed {MaxNameLength} characters.", nameof(category.Name));

            if (category.Description.Length > MaxDescriptionLength)
                throw new ArgumentException($"Category description cannot exceed {MaxDescriptionLength} characters.", nameof(category.Description));

            // EVAL: Validate attribute constraints (same column sizes as ProductAttributes)
            if (category.Attributes != null)
            {
                foreach (var attr in category.Attributes)
                {
                    if (string.IsNullOrWhiteSpace(attr.Key))
                        throw new ArgumentException("Attribute key cannot be empty.");
                    if (attr.Key.Length > 64)
                        throw new ArgumentException($"Attribute key '{attr.Key}' exceeds maximum length of 64 characters.");
                    if (attr.Value?.Length > 512)
                        throw new ArgumentException($"Attribute value for key '{attr.Key}' exceeds maximum length of 512 characters.");
                }
            }

            // EVAL: Validate parent categories exist before creating hierarchy links
            if (category.ParentCategoryIds?.Count > 0)
            {
                foreach (var parentId in category.ParentCategoryIds)
                {
                    var parent = await _CategoryRepository.GetByIdAsync(parentId);
                    if (parent == null)
                        throw new KeyNotFoundException($"Parent category with ID {parentId} does not exist.");
                }
            }

            return await _CategoryRepository.CreateAsync(category);
        }

        public async Task<Category?> GetCategoryByIdAsync(int instanceId)
        {
            if (instanceId <= 0)
                throw new ArgumentException("Instance ID must be a positive integer.", nameof(instanceId));

            return await _CategoryRepository.GetByIdAsync(instanceId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _CategoryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentCategoryId)
        {
            if (parentCategoryId <= 0)
                throw new ArgumentException("Parent category ID must be a positive integer.", nameof(parentCategoryId));

            return await _CategoryRepository.GetChildrenAsync(parentCategoryId);
        }
    }
}
