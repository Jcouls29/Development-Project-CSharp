// EVAL: Unit tests for ProductController using Moq to mock IProductRepository.
// Tests verify controller behavior (HTTP status codes, response mapping)
// independently of the database layer.
// Naming convention: MethodName_Scenario_ExpectedResult

using Interview.Web.Controllers;
using Interview.Web.Models.Requests;
using Interview.Web.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _controller = new ProductController(_mockRepo.Object);
        }

        #region CreateProduct Tests

        [Fact]
        public async Task CreateProduct_ValidRequest_Returns201Created()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                Description = "A test product",
                Skus = new List<string> { "SKU-001" },
                Attributes = new Dictionary<string, string> { { "Color", "Red" } },
                CategoryIds = new List<int> { 1 }
            };

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) =>
                {
                    p.InstanceId = 42;
                    p.CreatedTimestamp = System.DateTime.UtcNow;
                    return p;
                });

            // Act
            var result = await _controller.CreateProduct(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var response = Assert.IsType<ProductResponse>(createdResult.Value);
            Assert.Equal(42, response.InstanceId);
            Assert.Equal("Test Product", response.Name);
        }

        [Fact]
        public async Task CreateProduct_AttributeKeyTooLong_Returns400()
        {
            // Arrange
            // RV: Attribute key max is 64 chars (DB constraint)
            var longKey = new string('A', 65);
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                Description = "A test product",
                Attributes = new Dictionary<string, string> { { longKey, "Value" } }
            };

            // Act
            var result = await _controller.CreateProduct(request, CancellationToken.None);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(badRequest.Value);
            Assert.Equal("VALIDATION_ERROR", error.Code);
        }

        [Fact]
        public async Task CreateProduct_AttributeValueTooLong_Returns400()
        {
            // Arrange
            // RV: Attribute value max is 512 chars (DB constraint)
            var longValue = new string('B', 513);
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                Description = "A test product",
                Attributes = new Dictionary<string, string> { { "Color", longValue } }
            };

            // Act
            var result = await _controller.CreateProduct(request, CancellationToken.None);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(badRequest.Value);
            Assert.Equal("VALIDATION_ERROR", error.Code);
        }

        #endregion

        #region GetProductById Tests

        [Fact]
        public async Task GetProductById_ExistingProduct_Returns200()
        {
            // Arrange
            var product = new Product
            {
                InstanceId = 1,
                Name = "Galaxy S24",
                Description = "Samsung phone",
                ProductImageUris = new List<string> { "https://example.com/img.jpg" },
                ValidSkus = new List<string> { "SM-S928B" },
                Attributes = new Dictionary<string, string> { { "Brand", "Samsung" } },
                CategoryIds = new List<int> { 1, 4 }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductById(1, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.Equal(1, response.InstanceId);
            Assert.Equal("Galaxy S24", response.Name);
            Assert.Single(response.ImageUris);
            Assert.Equal("Samsung", response.Attributes["Brand"]);
        }

        [Fact]
        public async Task GetProductById_NonExistentProduct_Returns404()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(99999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetProductById(99999, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(notFound.Value);
            Assert.Equal("PRODUCT_NOT_FOUND", error.Code);
        }

        #endregion

        #region SearchProducts Tests

        [Fact]
        public async Task SearchProducts_NoFilters_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { InstanceId = 1, Name = "Product A", Description = "Desc A" },
                new Product { InstanceId = 2, Name = "Product B", Description = "Desc B" }
            };

            _mockRepo.Setup(r => r.SearchAsync(It.IsAny<ProductSearchCriteria>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var request = new SearchProductsRequest();

            // Act
            var result = await _controller.SearchProducts(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<ProductResponse>>(okResult.Value);
            Assert.Equal(2, response.Count);
        }

        [Fact]
        public async Task SearchProducts_NoResults_Returns200WithEmptyList()
        {
            // Arrange
            // EVAL: Empty search results return 200 with empty list, not 404.
            // 404 means the resource doesn't exist; an empty search result is valid.
            _mockRepo.Setup(r => r.SearchAsync(It.IsAny<ProductSearchCriteria>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var request = new SearchProductsRequest { Name = "NonexistentProduct" };

            // Act
            var result = await _controller.SearchProducts(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<ProductResponse>>(okResult.Value);
            Assert.Empty(response);
        }

        [Fact]
        public async Task SearchProducts_WithFilters_PassesCriteriaToRepository()
        {
            // Arrange
            ProductSearchCriteria? capturedCriteria = null;
            _mockRepo.Setup(r => r.SearchAsync(It.IsAny<ProductSearchCriteria>(), It.IsAny<CancellationToken>()))
                .Callback<ProductSearchCriteria, CancellationToken>((c, _) => capturedCriteria = c)
                .ReturnsAsync(new List<Product>());

            var request = new SearchProductsRequest
            {
                Name = "Galaxy",
                CategoryIds = new List<int> { 1, 4 },
                Attributes = new Dictionary<string, string> { { "Brand", "Samsung" } },
                Skip = 10,
                Take = 25
            };

            // Act
            await _controller.SearchProducts(request, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedCriteria);
            Assert.Equal("Galaxy", capturedCriteria!.Name);
            Assert.Equal(new List<int> { 1, 4 }, capturedCriteria.CategoryIds);
            Assert.Equal("Samsung", capturedCriteria.Attributes!["Brand"]);
            Assert.Equal(10, capturedCriteria.Skip);
            Assert.Equal(25, capturedCriteria.Take);
        }

        #endregion
    }
}
