using Sparcpoint.Inventory.IntegrationTests.Infrastructure;
using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.IntegrationTests.Tests
{
    [Collection(TestDatabaseCollection.Name)]
    public class ProductRepositoryTests : IAsyncLifetime
    {
        private readonly TestDatabaseFixture _Fixture;
        public ProductRepositoryTests(TestDatabaseFixture fixture) { _Fixture = fixture; }
        public Task InitializeAsync() => _Fixture.ResetAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task Create_Then_GetById_RoundTripsAllFields()
        {
            var repo = _Fixture.CreateProductRepository();

            var productId = await repo.CreateAsync(new CreateProductRequest
            {
                Name = "Acme Widget",
                Description = "A widget for acme purposes.",
                ImageUris = new[] { "https://cdn.example/widget-1.png", "https://cdn.example/widget-2.png" },
                ValidSkus = new[] { "ACME-001", "ACME-001-RED" },
                Attributes = new Dictionary<string, string>
                {
                    { "Color", "Red" },
                    { "Brand", "Acme" }
                }
            });

            var product = await repo.GetByIdAsync(productId);

            Assert.NotNull(product);
            Assert.Equal("Acme Widget", product.Name);
            Assert.Equal("A widget for acme purposes.", product.Description);
            Assert.Equal(2, product.ImageUris.Count);
            Assert.Equal(2, product.ValidSkus.Count);
            Assert.Equal("Red", product.Attributes["Color"]);
            Assert.Equal("Acme", product.Attributes["Brand"]);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenProductMissing()
        {
            var repo = _Fixture.CreateProductRepository();
            var product = await repo.GetByIdAsync(999999);
            Assert.Null(product);
        }

        [Fact]
        public async Task Search_ByAttribute_ReturnsOnlyMatchingProducts()
        {
            var repo = _Fixture.CreateProductRepository();

            await repo.CreateAsync(new CreateProductRequest
            {
                Name = "Red Widget",
                Description = "Red",
                Attributes = new Dictionary<string, string> { { "Color", "Red" } }
            });
            await repo.CreateAsync(new CreateProductRequest
            {
                Name = "Blue Widget",
                Description = "Blue",
                Attributes = new Dictionary<string, string> { { "Color", "Blue" } }
            });

            var results = (await repo.SearchAsync(new ProductSearchCriteria
            {
                AttributeFilters = new Dictionary<string, string> { { "Color", "Red" } }
            })).ToList();

            Assert.Single(results);
            Assert.Equal("Red Widget", results[0].Name);
        }

        [Fact]
        public async Task Search_ByCategory_ReturnsOnlyMatchingProducts()
        {
            var productRepo = _Fixture.CreateProductRepository();
            var categoryRepo = _Fixture.CreateCategoryRepository();

            var toolsCategoryId = await categoryRepo.CreateAsync(new CreateCategoryRequest { Name = "Tools" });
            var toysCategoryId = await categoryRepo.CreateAsync(new CreateCategoryRequest { Name = "Toys" });

            await productRepo.CreateAsync(new CreateProductRequest
            {
                Name = "Hammer",
                Description = "A hammer",
                CategoryIds = new[] { toolsCategoryId }
            });
            await productRepo.CreateAsync(new CreateProductRequest
            {
                Name = "Teddy Bear",
                Description = "A bear",
                CategoryIds = new[] { toysCategoryId }
            });

            var results = (await productRepo.SearchAsync(new ProductSearchCriteria
            {
                CategoryIds = new[] { toolsCategoryId }
            })).ToList();

            Assert.Single(results);
            Assert.Equal("Hammer", results[0].Name);
        }

        [Fact]
        public async Task Search_ByCategoryAndAttribute_AppliesAllFilters()
        {
            var productRepo = _Fixture.CreateProductRepository();
            var categoryRepo = _Fixture.CreateCategoryRepository();

            var paintsCategoryId = await categoryRepo.CreateAsync(new CreateCategoryRequest { Name = "Paints" });

            await productRepo.CreateAsync(new CreateProductRequest
            {
                Name = "Red Paint",
                Description = "Red",
                CategoryIds = new[] { paintsCategoryId },
                Attributes = new Dictionary<string, string> { { "Color", "Red" } }
            });
            await productRepo.CreateAsync(new CreateProductRequest
            {
                Name = "Blue Paint",
                Description = "Blue",
                CategoryIds = new[] { paintsCategoryId },
                Attributes = new Dictionary<string, string> { { "Color", "Blue" } }
            });

            var results = (await productRepo.SearchAsync(new ProductSearchCriteria
            {
                CategoryIds = new[] { paintsCategoryId },
                AttributeFilters = new Dictionary<string, string> { { "Color", "Red" } }
            })).ToList();

            Assert.Single(results);
            Assert.Equal("Red Paint", results[0].Name);
        }

        [Fact]
        public async Task Search_NoCriteria_ReturnsAllProducts()
        {
            var repo = _Fixture.CreateProductRepository();
            await repo.CreateAsync(new CreateProductRequest { Name = "P1", Description = "d" });
            await repo.CreateAsync(new CreateProductRequest { Name = "P2", Description = "d" });

            var results = await repo.SearchAsync(new ProductSearchCriteria());

            Assert.True(results.Count() >= 2);
        }

        [Fact]
        public async Task Create_ShouldTruncate_OvelyLongStrings()
        {
            // EVAL: Confirms the "string overruns" edge case the spec calls out is handled — a
            // 1000-char name is truncated to fit VARCHAR(256) rather than throwing SqlException.
            var repo = _Fixture.CreateProductRepository();
            var longName = new string('X', 1000);

            var id = await repo.CreateAsync(new CreateProductRequest
            {
                Name = longName,
                Description = "d"
            });

            var product = await repo.GetByIdAsync(id);
            Assert.Equal(256, product.Name.Length);
        }

        [Fact]
        public async Task Create_WithEmptyDescription_PersistsEmptyString()
        {
            var repo = _Fixture.CreateProductRepository();
            var id = await repo.CreateAsync(new CreateProductRequest { Name = "X", Description = null });
            var product = await repo.GetByIdAsync(id);
            Assert.Equal(string.Empty, product.Description);
        }
    }
}
