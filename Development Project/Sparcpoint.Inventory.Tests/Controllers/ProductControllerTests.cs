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
    /// By injecting the real service with in-memory repositories, we test
    /// the full request pipeline without a database.
    /// </summary>
    public class ProductControllerTests
    {
        private readonly ProductController _Controller;
        private readonly IProductService _ProductService;

        public ProductControllerTests()
        {
            var repository = new InMemoryProductRepository();
            _ProductService = new ProductService(repository);
            _Controller = new ProductController(_ProductService);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtAction()
        {
            var request = new CreateProductRequest
            {
                Name = "Widget",
                Description = "A fine widget",
                ValidSkus = new List<string> { "SKU-001" },
                Attributes = new Dictionary<string, string> { { "Color", "Blue" } }
            };

            var result = await _Controller.CreateProduct(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var response = Assert.IsType<ProductResponse>(createdResult.Value);
            Assert.Equal("Widget", response.Name);
            Assert.Contains("SKU-001", response.ValidSkus);
            Assert.Equal("Blue", response.Attributes["Color"]);
        }

        [Fact]
        public async Task GetProduct_WithExistingId_ReturnsOk()
        {
            // Create a product first
            var request = new CreateProductRequest
            {
                Name = "Findable",
                Description = "Can be found"
            };
            var createResult = await _Controller.CreateProduct(request) as CreatedAtActionResult;
            var created = createResult!.Value as ProductResponse;

            var result = await _Controller.GetProduct(created!.InstanceId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.Equal("Findable", response.Name);
        }

        [Fact]
        public async Task GetProduct_WithNonExistentId_ReturnsNotFound()
        {
            var result = await _Controller.GetProduct(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SearchProducts_WithNoCriteria_ReturnsAll()
        {
            await _Controller.CreateProduct(new CreateProductRequest { Name = "A", Description = "desc" });
            await _Controller.CreateProduct(new CreateProductRequest { Name = "B", Description = "desc" });

            var result = await _Controller.SearchProducts(new SearchProductsRequest());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductResponse>>(okResult.Value);
            Assert.Equal(2, products.Count);
        }

        [Fact]
        public async Task SearchProducts_ByName_ReturnsFiltered()
        {
            await _Controller.CreateProduct(new CreateProductRequest { Name = "Red Widget", Description = "desc" });
            await _Controller.CreateProduct(new CreateProductRequest { Name = "Blue Widget", Description = "desc" });
            await _Controller.CreateProduct(new CreateProductRequest { Name = "Red Gadget", Description = "desc" });

            var result = await _Controller.SearchProducts(new SearchProductsRequest { NameContains = "Red" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductResponse>>(okResult.Value);
            Assert.Equal(2, products.Count);
        }

        [Fact]
        public async Task SearchProducts_ByAttribute_ReturnsFiltered()
        {
            await _Controller.CreateProduct(new CreateProductRequest
            {
                Name = "P1", Description = "d",
                Attributes = new Dictionary<string, string> { { "Color", "Red" } }
            });
            await _Controller.CreateProduct(new CreateProductRequest
            {
                Name = "P2", Description = "d",
                Attributes = new Dictionary<string, string> { { "Color", "Blue" } }
            });

            var result = await _Controller.SearchProducts(new SearchProductsRequest
            {
                AttributeKey = "Color",
                AttributeValue = "Red"
            });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductResponse>>(okResult.Value);
            Assert.Single(products);
            Assert.Equal("P1", products[0].Name);
        }
    }
}
