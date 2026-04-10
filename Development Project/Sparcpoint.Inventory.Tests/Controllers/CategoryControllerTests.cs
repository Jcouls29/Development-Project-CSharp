using Interview.Web.Controllers;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Services;
using Sparcpoint.Inventory.Tests.Fakes;
using Xunit;

namespace Sparcpoint.Inventory.Tests.Controllers
{
    /// <summary>
    /// EVAL: Controller tests verify HTTP response codes and DTO mapping.
    /// By injecting the real service with an in-memory repository, we test
    /// the full request pipeline without a database.
    /// </summary>
    public class CategoryControllerTests
    {
        private readonly CategoryController _Controller;
        private readonly ICategoryService _CategoryService;

        public CategoryControllerTests()
        {
            var repository = new InMemoryCategoryRepository();
            _CategoryService = new CategoryService(repository);
            _Controller = new CategoryController(_CategoryService);
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedAtAction()
        {
            var request = new CreateCategoryRequest
            {
                Name = "Electronics",
                Description = "Electronic products",
                Attributes = new Dictionary<string, string> { { "Department", "Tech" } }
            };

            var result = await _Controller.CreateCategory(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var response = Assert.IsType<CategoryResponse>(createdResult.Value);
            Assert.Equal("Electronics", response.Name);
            Assert.Equal("Tech", response.Attributes["Department"]);
        }

        [Fact]
        public async Task GetCategory_WithExistingId_ReturnsOk()
        {
            var createResult = await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Findable",
                Description = "Can be found"
            }) as CreatedAtActionResult;
            var created = createResult!.Value as CategoryResponse;

            var result = await _Controller.GetCategory(created!.InstanceId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CategoryResponse>(okResult.Value);
            Assert.Equal("Findable", response.Name);
        }

        [Fact]
        public async Task GetCategory_WithNonExistentId_ReturnsNotFound()
        {
            var result = await _Controller.GetCategory(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsAllCategories()
        {
            await _Controller.CreateCategory(new CreateCategoryRequest { Name = "Alpha", Description = "First category" });
            await _Controller.CreateCategory(new CreateCategoryRequest { Name = "Beta", Description = "Second category" });

            var result = await _Controller.GetAllCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsAssignableFrom<IEnumerable<CategoryResponse>>(okResult.Value).ToList();
            Assert.Equal(2, categories.Count);
        }

        [Fact]
        public async Task GetChildCategories_ReturnsOnlyDirectChildren()
        {
            var parentResult = await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Parent",
                Description = "Root category"
            }) as CreatedAtActionResult;
            var parent = parentResult!.Value as CategoryResponse;

            await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Child1",
                Description = "First child",
                ParentCategoryIds = new List<int> { parent!.InstanceId }
            });
            await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Child2",
                Description = "Second child",
                ParentCategoryIds = new List<int> { parent.InstanceId }
            });
            await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Unrelated",
                Description = "No parent"
            });

            var result = await _Controller.GetChildCategories(parent.InstanceId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var children = Assert.IsAssignableFrom<IEnumerable<CategoryResponse>>(okResult.Value).ToList();
            Assert.Equal(2, children.Count);
            Assert.All(children, c => Assert.Contains(parent.InstanceId, c.ParentCategoryIds));
        }

        [Fact]
        public async Task GetChildCategories_WithNoChildren_ReturnsEmptyList()
        {
            var parentResult = await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Leaf",
                Description = "No children"
            }) as CreatedAtActionResult;
            var parent = parentResult!.Value as CategoryResponse;

            var result = await _Controller.GetChildCategories(parent!.InstanceId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var children = Assert.IsAssignableFrom<IEnumerable<CategoryResponse>>(okResult.Value).ToList();
            Assert.Empty(children);
        }

        [Fact]
        public async Task CreateCategory_WithParentIds_ReturnsCreatedWithParentIds()
        {
            var parentResult = await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Parent",
                Description = "Root category"
            }) as CreatedAtActionResult;
            var parent = parentResult!.Value as CategoryResponse;

            var result = await _Controller.CreateCategory(new CreateCategoryRequest
            {
                Name = "Child",
                Description = "Child category",
                ParentCategoryIds = new List<int> { parent!.InstanceId }
            });

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<CategoryResponse>(createdResult.Value);
            Assert.Contains(parent.InstanceId, response.ParentCategoryIds);
        }
    }
}
