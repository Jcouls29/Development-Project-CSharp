// EVAL: Integration-style tests using InMemoryProductRepository.
// These test the full repository contract with real data flowing through,
// not just mocked return values. Proves the interface is correctly designed
// and exercisable without SQL Server.

using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Tests.Fakes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Tests
{
    public class ProductRepositoryTests
    {
        private readonly InMemoryProductRepository _repo;

        public ProductRepositoryTests()
        {
            _repo = new InMemoryProductRepository();
            SeedTestData();
        }

        private void SeedTestData()
        {
            _repo.Seed(new Product
            {
                InstanceId = 1,
                Name = "Galaxy S24 Ultra",
                Description = "Samsung flagship smartphone",
                ProductImageUris = new List<string> { "https://example.com/galaxy.jpg" },
                ValidSkus = new List<string> { "SM-S928B" },
                Attributes = new Dictionary<string, string>
                {
                    { "Brand", "Samsung" }, { "Color", "Black" }, { "Storage", "256GB" }
                },
                CategoryIds = new List<int> { 1, 4 },
                Categories = new Dictionary<int, string> { { 1, "Electronics" }, { 4, "Phones" } }
            });

            _repo.Seed(new Product
            {
                InstanceId = 2,
                Name = "MacBook Pro 16\"",
                Description = "Apple laptop with M3 Pro chip",
                ProductImageUris = new List<string> { "https://example.com/macbook.jpg" },
                ValidSkus = new List<string> { "MRW13LL/A" },
                Attributes = new Dictionary<string, string>
                {
                    { "Brand", "Apple" }, { "Color", "Space Black" }, { "Storage", "512GB" }
                },
                CategoryIds = new List<int> { 1, 5 },
                Categories = new Dictionary<int, string> { { 1, "Electronics" }, { 5, "Laptops" } }
            });

            _repo.Seed(new Product
            {
                InstanceId = 3,
                Name = "Air Max 90",
                Description = "Nike classic running shoe",
                Attributes = new Dictionary<string, string>
                {
                    { "Brand", "Nike" }, { "Color", "White" }, { "Size", "10" }
                },
                CategoryIds = new List<int> { 2, 6 },
                Categories = new Dictionary<int, string> { { 2, "Clothing" }, { 6, "Shoes" } }
            });
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidProduct_AssignsIdAndReturns()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "A test product",
                Attributes = new Dictionary<string, string> { { "Color", "Red" } },
                CategoryIds = new List<int> { 1 }
            };

            var created = await _repo.CreateAsync(product);

            Assert.True(created.InstanceId > 0);
            Assert.Equal("Test Product", created.Name);
            Assert.Equal("Red", created.Attributes["Color"]);
        }

        [Fact]
        public async Task CreateAsync_MultipleCalls_AssignUniqueIds()
        {
            var p1 = await _repo.CreateAsync(new Product { Name = "P1", Description = "D1" });
            var p2 = await _repo.CreateAsync(new Product { Name = "P2", Description = "D2" });

            Assert.NotEqual(p1.InstanceId, p2.InstanceId);
        }

        [Fact]
        public async Task CreateAsync_ProductRetrievableAfterCreation()
        {
            var created = await _repo.CreateAsync(new Product
            {
                Name = "Retrievable Product",
                Description = "Should be findable",
                Attributes = new Dictionary<string, string> { { "Brand", "TestBrand" } }
            });

            var retrieved = await _repo.GetByIdAsync(created.InstanceId);

            Assert.NotNull(retrieved);
            Assert.Equal("Retrievable Product", retrieved.Name);
            Assert.Equal("TestBrand", retrieved.Attributes["Brand"]);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ExistingProduct_ReturnsWithAllData()
        {
            var product = await _repo.GetByIdAsync(1);

            Assert.NotNull(product);
            Assert.Equal("Galaxy S24 Ultra", product.Name);
            Assert.Equal("Samsung", product.Attributes["Brand"]);
            Assert.Contains(4, product.CategoryIds);
            Assert.Single(product.ProductImageUris);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            var product = await _repo.GetByIdAsync(99999);
            Assert.Null(product);
        }

        #endregion

        #region SearchAsync Tests

        [Fact]
        public async Task SearchAsync_NoFilters_ReturnsAll()
        {
            var results = await _repo.SearchAsync(new ProductSearchCriteria());

            Assert.Equal(3, results.Count());
        }

        [Fact]
        public async Task SearchAsync_ByName_PartialMatch()
        {
            var results = await _repo.SearchAsync(new ProductSearchCriteria { Name = "Galaxy" });

            Assert.Single(results);
            Assert.Equal("Galaxy S24 Ultra", results.First().Name);
        }

        [Fact]
        public async Task SearchAsync_ByName_CaseInsensitive()
        {
            var results = await _repo.SearchAsync(new ProductSearchCriteria { Name = "galaxy" });

            Assert.Single(results);
        }

        [Fact]
        public async Task SearchAsync_ByCategory_ReturnsMatchingProducts()
        {
            // Category 1 = Electronics (Galaxy + MacBook)
            var results = await _repo.SearchAsync(new ProductSearchCriteria
            {
                CategoryIds = new List<int> { 1 }
            });

            Assert.Equal(2, results.Count());
            Assert.All(results, p => Assert.Contains(1, p.CategoryIds));
        }

        [Fact]
        public async Task SearchAsync_ByMultipleCategories_ReturnsUnion()
        {
            // Category 4 = Phones, Category 6 = Shoes
            var results = await _repo.SearchAsync(new ProductSearchCriteria
            {
                CategoryIds = new List<int> { 4, 6 }
            });

            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task SearchAsync_ByAttribute_ExactMatch()
        {
            var results = await _repo.SearchAsync(new ProductSearchCriteria
            {
                Attributes = new Dictionary<string, string> { { "Brand", "Samsung" } }
            });

            Assert.Single(results);
            Assert.Equal("Galaxy S24 Ultra", results.First().Name);
        }

        [Fact]
        public async Task SearchAsync_ByMultipleAttributes_AndLogic()
        {
            // Brand=Samsung AND Color=Black -- should match Galaxy
            var results = await _repo.SearchAsync(new ProductSearchCriteria
            {
                Attributes = new Dictionary<string, string>
                {
                    { "Brand", "Samsung" },
                    { "Color", "Black" }
                }
            });

            Assert.Single(results);
        }

        [Fact]
        public async Task SearchAsync_ByMultipleAttributes_NoMatch()
        {
            // Brand=Samsung AND Color=White -- no product has both
            var results = await _repo.SearchAsync(new ProductSearchCriteria
            {
                Attributes = new Dictionary<string, string>
                {
                    { "Brand", "Samsung" },
                    { "Color", "White" }
                }
            });

            Assert.Empty(results);
        }

        [Fact]
        public async Task SearchAsync_CombinedFilters_NameAndCategory()
        {
            var results = await _repo.SearchAsync(new ProductSearchCriteria
            {
                Name = "Mac",
                CategoryIds = new List<int> { 1 }
            });

            Assert.Single(results);
            Assert.Equal("MacBook Pro 16\"", results.First().Name);
        }

        [Fact]
        public async Task SearchAsync_Pagination_SkipAndTake()
        {
            var page1 = await _repo.SearchAsync(new ProductSearchCriteria { Skip = 0, Take = 2 });
            var page2 = await _repo.SearchAsync(new ProductSearchCriteria { Skip = 2, Take = 2 });

            Assert.Equal(2, page1.Count());
            Assert.Single(page2);
            // No overlap
            Assert.Empty(page1.Select(p => p.InstanceId).Intersect(page2.Select(p => p.InstanceId)));
        }

        [Fact]
        public async Task SearchAsync_NoMatch_ReturnsEmptyNotNull()
        {
            var results = await _repo.SearchAsync(new ProductSearchCriteria { Name = "NonexistentXYZ" });

            Assert.NotNull(results);
            Assert.Empty(results);
        }

        #endregion
    }
}
