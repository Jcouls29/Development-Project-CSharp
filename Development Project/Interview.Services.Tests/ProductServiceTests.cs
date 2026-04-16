using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Moq;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Interview.Services.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<ISqlExecutor> _mockSqlExecutor;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockSqlExecutor = new Mock<ISqlExecutor>();
            _productService = new ProductService(_mockSqlExecutor.Object);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productService.CreateProductAsync(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateProductAsync_ShouldThrowArgumentException_WhenNameIsInvalid(string name)
        {
            var request = CreateValidProductRequest();
            request.Name = name;

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(request));
            Assert.Contains("Product Name is required", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateProductAsync_ShouldThrowArgumentException_WhenDescriptionIsInvalid(string description)
        {
            var request = CreateValidProductRequest();
            request.Description = description;

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(request));
            Assert.Contains("Product Description is required", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldThrowArgumentException_WhenMetadataHasSpecialCharacters()
        {
            var request = CreateValidProductRequest();
            request.Metadata = new Dictionary<string, string> { { "Key!", "Value" } };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(request));
            Assert.Contains("contains special characters", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCallSqlExecutor_WhenRequestIsValid()
        {
            // Arrange
            var request = CreateValidProductRequest();
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(1);

            // Act
            var result = await _productService.CreateProductAsync(request);

            // Assert
            Assert.Equal(1, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()), Times.Once);
        }

        [Fact]
        public async Task SearchProductsAsync_ShouldCallSqlExecutor()
        {
            // Arrange
            var request = new ProductSearchRequest();
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<IEnumerable<ProductResponse>>>>()))
                .ReturnsAsync(new List<ProductResponse>());

            // Act
            await _productService.SearchProductsAsync(request);

            // Assert
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<IEnumerable<ProductResponse>>>>()), Times.Once);
        }

        private ProductRequest CreateValidProductRequest()
        {
            return new ProductRequest
            {
                Name = "Test Product",
                Description = "Test Description",
                ProductImageUris = "http://image.com",
                ValidSkus = "SKU1,SKU2",
                Metadata = new Dictionary<string, string>()
            };
        }
    }
}
