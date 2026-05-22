using Sparcpoint.Inventory.IntegrationTests.Infrastructure;
using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.IntegrationTests.Tests
{
    [Collection(TestDatabaseCollection.Name)]
    public class CategoryRepositoryTests : IAsyncLifetime
    {
        private readonly TestDatabaseFixture _Fixture;
        public CategoryRepositoryTests(TestDatabaseFixture fixture) { _Fixture = fixture; }
        public Task InitializeAsync() => _Fixture.ResetAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task Create_Then_GetById_RoundTripsAllFields()
        {
            var repo = _Fixture.CreateCategoryRepository();

            var id = await repo.CreateAsync(new CreateCategoryRequest
            {
                Name = "Hardware",
                Description = "Top-level hardware",
                Attributes = new Dictionary<string, string> { { "ShelfPrefix", "HW" } }
            });

            var category = await repo.GetByIdAsync(id);

            Assert.NotNull(category);
            Assert.Equal("Hardware", category.Name);
            Assert.Equal("HW", category.Attributes["ShelfPrefix"]);
        }

        [Fact]
        public async Task Create_WithParentCategories_PersistsHierarchy()
        {
            // EVAL: Verifies requirement on hierarchies (Goal #3). A child category can reference
            // multiple parents via [Instances].[CategoryCategories].
            var repo = _Fixture.CreateCategoryRepository();

            var parentId = await repo.CreateAsync(new CreateCategoryRequest { Name = "Hardware" });
            var childId = await repo.CreateAsync(new CreateCategoryRequest
            {
                Name = "Power Tools",
                ParentCategoryIds = new[] { parentId }
            });

            var child = await repo.GetByIdAsync(childId);
            Assert.Contains(parentId, child.ParentCategoryIds);
        }

        [Fact]
        public async Task GetAll_ReturnsAllCategories()
        {
            var repo = _Fixture.CreateCategoryRepository();
            await repo.CreateAsync(new CreateCategoryRequest { Name = "Cat A" });
            await repo.CreateAsync(new CreateCategoryRequest { Name = "Cat B" });

            var all = (await repo.GetAllAsync()).ToList();

            Assert.True(all.Count >= 2);
            Assert.Contains(all, c => c.Name == "Cat A");
            Assert.Contains(all, c => c.Name == "Cat B");
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenMissing()
        {
            var repo = _Fixture.CreateCategoryRepository();
            Assert.Null(await repo.GetByIdAsync(999999));
        }
    }
}
