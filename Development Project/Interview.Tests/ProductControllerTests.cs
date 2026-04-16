using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Interview.Web.Models;
using Interview.Web.Repositories;
using Interview.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Interview.Tests
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

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtAction_WhenValid()
        {
            // Arrange
            var request = new CreateProductRequest { Name = "Test Product" };
            _mockRepo.Setup(r => r.AddProductAsync(request)).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateProduct(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }

        [Fact]
        public async Task SearchProducts_ReturnsOk_WithResults()
        {
            // Arrange
            var products = new List<ProductResponse> { new ProductResponse { Name = "Test" } };
            _mockRepo.Setup(r => r.SearchProductsAsync(null, null, null)).ReturnsAsync(products);

            // Act
            var result = await _controller.SearchProducts(null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
            Assert.Single(returnedProducts);
        }
    }
}
