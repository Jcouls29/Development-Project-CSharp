using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;
using Sparcpoint.Inventory.Tests.Fakes;
using Xunit;

namespace Sparcpoint.Inventory.Tests.Services
{
    /// <summary>
    /// EVAL: Unit tests for CategoryService demonstrating that business rules
    /// are enforced independently of the data layer.
    /// </summary>
    public class CategoryServiceTests
    {
        private readonly InMemoryCategoryRepository _Repository;
        private readonly CategoryService _Service;

        public CategoryServiceTests()
        {
            _Repository = new InMemoryCategoryRepository();
            _Service = new CategoryService(_Repository);
        }

        [Fact]
        public async Task CreateCategory_WithValidData_ReturnsCategoryWithId()
        {
            var category = new Category
            {
                Name = "Electronics",
                Description = "Electronic products",
                Attributes = new Dictionary<string, string> { { "Department", "Tech" } }
            };

            var result = await _Service.CreateCategoryAsync(category);

            Assert.True(result.InstanceId > 0);
            Assert.Equal("Electronics", result.Name);
            Assert.Equal("Electronic products", result.Description);
            Assert.Equal("Tech", result.Attributes["Department"]);
        }

        [Fact]
        public async Task CreateCategory_WithNullCategory_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Service.CreateCategoryAsync(null!));
        }

        [Fact]
        public async Task CreateCategory_WithEmptyName_ThrowsArgumentException()
        {
            var category = new Category
            {
                Name = "",
                Description = "Valid description"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateCategoryAsync(category));
        }

        [Fact]
        public async Task CreateCategory_WithEmptyDescription_ThrowsArgumentException()
        {
            var category = new Category
            {
                Name = "Valid Name",
                Description = ""
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateCategoryAsync(category));
        }

        [Fact]
        public async Task CreateCategory_WithNameExceedingMaxLength_ThrowsArgumentException()
        {
            var category = new Category
            {
                Name = new string('A', 65),
                Description = "Valid description"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateCategoryAsync(category));
            Assert.Contains("64", ex.Message);
        }

        [Fact]
        public async Task CreateCategory_WithDescriptionExceedingMaxLength_ThrowsArgumentException()
        {
            var category = new Category
            {
                Name = "Valid Name",
                Description = new string('D', 257)
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateCategoryAsync(category));
            Assert.Contains("256", ex.Message);
        }

        [Fact]
        public async Task CreateCategory_WithAttributeKeyTooLong_ThrowsArgumentException()
        {
            var category = new Category
            {
                Name = "Valid Name",
                Description = "Valid description",
                Attributes = new Dictionary<string, string>
                {
                    { new string('K', 65), "value" }
                }
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateCategoryAsync(category));
            Assert.Contains("64", ex.Message);
        }

        [Fact]
        public async Task CreateCategory_WithAttributeValueTooLong_ThrowsArgumentException()
        {
            var category = new Category
            {
                Name = "Valid Name",
                Description = "Valid description",
                Attributes = new Dictionary<string, string>
                {
                    { "Key", new string('V', 513) }
                }
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateCategoryAsync(category));
            Assert.Contains("512", ex.Message);
        }

        [Fact]
        public async Task CreateCategory_WithEmptyAttributeKey_ThrowsArgumentException()
        {
            var category = new Category
            {
                Name = "Valid Name",
                Description = "Valid description",
                Attributes = new Dictionary<string, string>
                {
                    { " ", "value" }
                }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateCategoryAsync(category));
        }

        [Fact]
        public async Task CreateCategory_WithValidParentCategoryIds_ReturnsCategory()
        {
            var parent = await _Service.CreateCategoryAsync(new Category
            {
                Name = "Parent",
                Description = "Parent category"
            });

            var child = new Category
            {
                Name = "Child",
                Description = "Child category",
                ParentCategoryIds = new List<int> { parent.InstanceId }
            };

            var result = await _Service.CreateCategoryAsync(child);

            Assert.True(result.InstanceId > 0);
            Assert.Contains(parent.InstanceId, result.ParentCategoryIds);
        }

        [Fact]
        public async Task CreateCategory_WithNonExistentParentId_ThrowsKeyNotFoundException()
        {
            var category = new Category
            {
                Name = "Orphan",
                Description = "Category with invalid parent",
                ParentCategoryIds = new List<int> { 9999 }
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _Service.CreateCategoryAsync(category));
        }

        [Fact]
        public async Task GetCategoryById_WithValidId_ReturnsCategory()
        {
            var created = await _Service.CreateCategoryAsync(new Category
            {
                Name = "Findable",
                Description = "Should be found"
            });

            var result = await _Service.GetCategoryByIdAsync(created.InstanceId);

            Assert.NotNull(result);
            Assert.Equal("Findable", result!.Name);
        }

        [Fact]
        public async Task GetCategoryById_WithNonExistentId_ReturnsNull()
        {
            var result = await _Service.GetCategoryByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCategoryById_WithZeroId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _Service.GetCategoryByIdAsync(0));
        }

        [Fact]
        public async Task GetAllCategories_ReturnsAllCategories()
        {
            await _Service.CreateCategoryAsync(new Category { Name = "Alpha", Description = "First" });
            await _Service.CreateCategoryAsync(new Category { Name = "Beta", Description = "Second" });

            var results = (await _Service.GetAllCategoriesAsync()).ToList();

            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetChildCategories_ReturnsOnlyDirectChildren()
        {
            var parent = await _Service.CreateCategoryAsync(new Category { Name = "Parent", Description = "Root" });
            await _Service.CreateCategoryAsync(new Category
            {
                Name = "Child1",
                Description = "First child",
                ParentCategoryIds = new List<int> { parent.InstanceId }
            });
            await _Service.CreateCategoryAsync(new Category
            {
                Name = "Child2",
                Description = "Second child",
                ParentCategoryIds = new List<int> { parent.InstanceId }
            });
            await _Service.CreateCategoryAsync(new Category { Name = "Unrelated", Description = "No parent link" });

            var children = (await _Service.GetChildCategoriesAsync(parent.InstanceId)).ToList();

            Assert.Equal(2, children.Count);
            Assert.All(children, c => Assert.Contains(parent.InstanceId, c.ParentCategoryIds));
        }

        [Fact]
        public async Task GetChildCategories_WithZeroParentId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _Service.GetChildCategoriesAsync(0));
        }

        [Fact]
        public async Task GetChildCategories_WithNoChildren_ReturnsEmptyList()
        {
            var parent = await _Service.CreateCategoryAsync(new Category { Name = "Leaf", Description = "No children" });

            var children = (await _Service.GetChildCategoriesAsync(parent.InstanceId)).ToList();

            Assert.Empty(children);
        }
    }
}
