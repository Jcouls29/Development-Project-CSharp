using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories.InMemory
{
    /// <summary>
    /// EVAL: Thread-safe in-memory implementation of ICategoryRepository.
    /// Supports hierarchical categories for development and testing.
    /// </summary>
    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _Categories = new();
        private readonly object _Lock = new();
        private int _NextId = 1;

        public Task<Category> CreateAsync(Category category)
        {
            lock (_Lock)
            {
                category.InstanceId = _NextId++;
                category.CreatedTimestamp = DateTime.UtcNow;
                _Categories.Add(CloneCategory(category));
            }

            return Task.FromResult(category);
        }

        public Task<Category?> GetByIdAsync(int instanceId)
        {
            lock (_Lock)
            {
                var category = _Categories.FirstOrDefault(c => c.InstanceId == instanceId);
                return Task.FromResult(category != null ? CloneCategory(category) : null);
            }
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            lock (_Lock)
            {
                return Task.FromResult<IEnumerable<Category>>(
                    _Categories.Select(CloneCategory).OrderBy(c => c.Name).ToList());
            }
        }

        public Task<IEnumerable<Category>> GetChildrenAsync(int parentCategoryId)
        {
            lock (_Lock)
            {
                var children = _Categories
                    .Where(c => c.ParentCategoryIds.Contains(parentCategoryId))
                    .Select(CloneCategory)
                    .OrderBy(c => c.Name)
                    .ToList();

                return Task.FromResult<IEnumerable<Category>>(children);
            }
        }

        private static Category CloneCategory(Category source)
        {
            return new Category
            {
                InstanceId = source.InstanceId,
                Name = source.Name,
                Description = source.Description,
                CreatedTimestamp = source.CreatedTimestamp,
                Attributes = source.Attributes != null ? new Dictionary<string, string>(source.Attributes) : new(),
                ParentCategoryIds = source.ParentCategoryIds != null ? new List<int>(source.ParentCategoryIds) : new()
            };
        }
    }
}
