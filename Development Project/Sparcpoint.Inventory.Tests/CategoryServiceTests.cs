using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories.InMemory;
using Sparcpoint.Inventory.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class CategoryServiceTests
    {
        private static CategoryService CreateService() => new(new InMemoryCategoryRepository());

        [Fact]
        public async Task Create_Then_Get_RoundTrips()
        {
            var service = CreateService();
            var id = await service.CreateAsync(new Category
            {
                Name = "Tools",
                Description = "Hand tools",
                Attributes = new[] { new ProductAttribute("Dept", "Hardware") }
            });

            var fetched = await service.GetAsync(id);
            Assert.NotNull(fetched);
            Assert.Equal("Tools", fetched!.Name);
            Assert.Single(fetched.Attributes);
        }

        [Fact]
        public async Task Create_Missing_Name_Throws()
        {
            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new Category { Name = "" }));
        }

        [Fact]
        public async Task Update_Returns_True_When_Found()
        {
            var service = CreateService();
            var id = await service.CreateAsync(new Category { Name = "A" });

            var ok = await service.UpdateAsync(new Category { InstanceId = id, Name = "A2", Description = "renamed" });
            Assert.True(ok);

            var fetched = await service.GetAsync(id);
            Assert.Equal("A2", fetched!.Name);
            Assert.Equal("renamed", fetched.Description);
        }

        [Fact]
        public async Task Update_Missing_Returns_False()
        {
            var service = CreateService();
            Assert.False(await service.UpdateAsync(new Category { InstanceId = 999, Name = "x" }));
        }

        [Fact]
        public async Task Delete_Returns_True_And_Removes()
        {
            var service = CreateService();
            var id = await service.CreateAsync(new Category { Name = "A" });
            Assert.True(await service.DeleteAsync(id));
            Assert.Null(await service.GetAsync(id));
        }

        [Fact]
        public async Task Descendants_Walks_Hierarchy()
        {
            var service = CreateService();
            var root = await service.CreateAsync(new Category { Name = "Root" });
            var childA = await service.CreateAsync(new Category { Name = "A", ParentCategoryIds = new[] { root } });
            var childB = await service.CreateAsync(new Category { Name = "B", ParentCategoryIds = new[] { root } });
            var grand = await service.CreateAsync(new Category { Name = "A1", ParentCategoryIds = new[] { childA } });

            var descendants = await service.GetDescendantsAsync(root);
            var ids = descendants.Select(c => c.InstanceId).OrderBy(x => x).ToArray();
            Assert.Equal(new[] { childA, childB, grand }.OrderBy(x => x).ToArray(), ids);
        }

        [Fact]
        public async Task List_Returns_All_Created()
        {
            var service = CreateService();
            await service.CreateAsync(new Category { Name = "A" });
            await service.CreateAsync(new Category { Name = "B" });
            var all = await service.ListAsync();
            Assert.Equal(2, all.Count);
        }
    }
}
