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


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Create_ShouldFail_On_Empty_Name(string name)
        {
            var req = CreateValidProductRequest();
            req.Name = name;

            var err = await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(req));
            Assert.Contains("Product Name is required", err.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Create_ShouldFail_On_Empty_Description(string description)
        {
            var req = CreateValidProductRequest();
            req.Description = description;

            var err = await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(req));
            Assert.Contains("Product Description is required", err.Message);
        }

        [Fact]
        public async Task Create_ShouldReject_SpecialChars_In_Metadata()
        {
            var req = CreateValidProductRequest();
            req.Metadata = new Dictionary<string, string> { { "Invalid Key!", "Value" } };

            var err = await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(req));
            Assert.Contains("contains special characters", err.Message);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCallSqlExecutor_WhenRequestIsValid()
        {
            var request = CreateValidProductRequest();
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(1);

            var result = await _productService.CreateProductAsync(request);

            Assert.Equal(1, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldPropagateData_And_RefreshMetadata()
        {
            var req = CreateValidProductRequest();
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            await _productService.UpdateProductAsync(10, req);

            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()), Times.Once);
        }


        [Fact]
        public async Task SearchProductsAsync_ShouldInvokeExecutor()
        {
            var request = new ProductSearchRequest();
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<IEnumerable<ProductResponse>>>>()))
                .ReturnsAsync(new List<ProductResponse>());

            var results = await _productService.SearchProductsAsync(request);

            Assert.NotNull(results);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<IEnumerable<ProductResponse>>>>()), Times.Once);
        }

        private ProductRequest CreateValidProductRequest()
        {
            return new ProductRequest
            {
                Name = "Unit Test Product",
                Description = "A product for testing purposes",
                ProductImageUris = "http://test.com/img.png",
                ValidSkus = "TEST-01,TEST-02",
                Metadata = new Dictionary<string, string> { { "Environment", "Test" } }
            };
        }
    }
}
