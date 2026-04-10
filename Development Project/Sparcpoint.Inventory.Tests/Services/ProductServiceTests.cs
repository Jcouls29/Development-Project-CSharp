using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;
using Sparcpoint.Inventory.Tests.Fakes;
using Xunit;

namespace Sparcpoint.Inventory.Tests.Services
{
    /// <summary>
    /// EVAL: Unit tests for ProductService demonstrating that business rules
    /// are enforced independently of the data layer.
    /// </summary>
    public class ProductServiceTests
    {
        private readonly InMemoryProductRepository _Repository;
        private readonly ProductService _Service;

        public ProductServiceTests()
        {
            _Repository = new InMemoryProductRepository();
            _Service = new ProductService(_Repository);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsProductWithId()
        {
            var product = new Product
            {
                Name = "Test Widget",
                Description = "A test product",
                ProductImageUris = "",
                ValidSkus = "SKU-001,SKU-002",
                Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Brand", "Acme" } },
                CategoryIds = new List<int> { 1, 2 }
            };

            var result = await _Service.CreateProductAsync(product);

            Assert.True(result.InstanceId > 0);
            Assert.Equal("Test Widget", result.Name);
            Assert.Equal(2, result.Attributes.Count);
            Assert.Equal(2, result.CategoryIds.Count);
        }

        [Fact]
        public async Task CreateProduct_WithNullName_ThrowsArgumentException()
        {
            var product = new Product
            {
                Name = "",
                Description = "A test product",
                ProductImageUris = "",
                ValidSkus = ""
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateProductAsync(product));
        }

        [Fact]
        public async Task CreateProduct_WithNullDescription_ThrowsArgumentException()
        {
            var product = new Product
            {
                Name = "Valid Name",
                Description = "",
                ProductImageUris = "",
                ValidSkus = ""
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateProductAsync(product));
        }

        [Fact]
        public async Task CreateProduct_WithNameExceedingMaxLength_ThrowsArgumentException()
        {
            var product = new Product
            {
                Name = new string('A', 257),
                Description = "Valid description",
                ProductImageUris = "",
                ValidSkus = ""
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateProductAsync(product));
            Assert.Contains("256", ex.Message);
        }

        [Fact]
        public async Task CreateProduct_WithAttributeKeyTooLong_ThrowsArgumentException()
        {
            var product = new Product
            {
                Name = "Valid Name",
                Description = "Valid description",
                ProductImageUris = "",
                ValidSkus = "",
                Attributes = new Dictionary<string, string>
                {
                    { new string('K', 65), "value" }
                }
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateProductAsync(product));
            Assert.Contains("64", ex.Message);
        }

        [Fact]
        public async Task CreateProduct_WithAttributeValueTooLong_ThrowsArgumentException()
        {
            var product = new Product
            {
                Name = "Valid Name",
                Description = "Valid description",
                ProductImageUris = "",
                ValidSkus = "",
                Attributes = new Dictionary<string, string>
                {
                    { "Color", new string('V', 513) }
                }
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _Service.CreateProductAsync(product));
            Assert.Contains("512", ex.Message);
        }

        [Fact]
        public async Task CreateProduct_WithNullProduct_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Service.CreateProductAsync(null!));
        }

        [Fact]
        public async Task GetProductById_WithValidId_ReturnsProduct()
        {
            var product = new Product
            {
                Name = "Findable Product",
                Description = "Should be found",
                ProductImageUris = "",
                ValidSkus = ""
            };
            var created = await _Service.CreateProductAsync(product);

            var result = await _Service.GetProductByIdAsync(created.InstanceId);

            Assert.NotNull(result);
            Assert.Equal("Findable Product", result!.Name);
        }

        [Fact]
        public async Task GetProductById_WithInvalidId_ReturnsNull()
        {
            var result = await _Service.GetProductByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_WithZeroId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _Service.GetProductByIdAsync(0));
        }

        [Fact]
        public async Task SearchProducts_ByName_ReturnsMatchingProducts()
        {
            await CreateSampleProduct("Blue Widget", "A blue widget", new Dictionary<string, string> { { "Color", "Blue" } });
            await CreateSampleProduct("Red Gadget", "A red gadget", new Dictionary<string, string> { { "Color", "Red" } });
            await CreateSampleProduct("Blue Gadget", "A blue gadget", new Dictionary<string, string> { { "Color", "Blue" } });

            var criteria = new ProductSearchCriteria { NameContains = "Blue" };
            var results = (await _Service.SearchProductsAsync(criteria)).ToList();

            Assert.Equal(2, results.Count);
            Assert.All(results, r => Assert.Contains("Blue", r.Name));
        }

        [Fact]
        public async Task SearchProducts_ByAttribute_ReturnsMatchingProducts()
        {
            await CreateSampleProduct("Product A", "Desc A", new Dictionary<string, string> { { "Color", "Red" } });
            await CreateSampleProduct("Product B", "Desc B", new Dictionary<string, string> { { "Color", "Blue" } });
            await CreateSampleProduct("Product C", "Desc C", new Dictionary<string, string> { { "Color", "Red" } });

            var criteria = new ProductSearchCriteria
            {
                Attributes = new Dictionary<string, string> { { "Color", "Red" } }
            };
            var results = (await _Service.SearchProductsAsync(criteria)).ToList();

            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task SearchProducts_WithNoCriteria_ReturnsAll()
        {
            await CreateSampleProduct("Product A", "Desc A");
            await CreateSampleProduct("Product B", "Desc B");

            var criteria = new ProductSearchCriteria();
            var results = (await _Service.SearchProductsAsync(criteria)).ToList();

            Assert.Equal(2, results.Count);
        }

        private async Task<Product> CreateSampleProduct(string name, string description, Dictionary<string, string>? attributes = null)
        {
            return await _Service.CreateProductAsync(new Product
            {
                Name = name,
                Description = description,
                ProductImageUris = "",
                ValidSkus = "",
                Attributes = attributes ?? new Dictionary<string, string>()
            });
        }
    }
}
