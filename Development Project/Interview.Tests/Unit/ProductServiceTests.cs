using Moq;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Interview.Tests
{
    public class ProductServiceTests
    {
        #region CreateProductAsync

        [Fact]
        public async Task CreateProductAsync_ValidRequest_ReturnsProduct()
        {
            var request = new CreateProductRequest
            {
                Name = "Widget",
                Description = "A test widget",
            };

            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.CreateAsync(request)).ReturnsAsync(1);
            mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new ProductDetailModel
                {
                    InstanceId = 1,
                    Name = request.Name,
                    Description = request.Description,
                });

            var service = new ProductService(mockRepo.Object);

            var result = await service.CreateProductAsync(request);

            Assert.NotNull(result);
            Assert.Equal("Widget", result.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateProductAsync_NullOrWhitespaceName_ThrowsArgumentException(string? name)
        {
            var request = new CreateProductRequest
            {
                Name = name,
                Description = "Description",
            };

            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateProductAsync(request));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateProductAsync_NullOrWhitespaceDescription_ThrowsArgumentException(string? description)
        {
            var request = new CreateProductRequest
            {
                Name = "Name",
                Description = description,
            };

            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateProductAsync(request));
        }

        [Fact]
        public async Task CreateProductAsync_NullRequest_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateProductAsync(null!));
        }

        #endregion

        #region GetProductAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetProductAsync_InvalidId_ThrowsArgumentOutOfRange(int invalidId)
        {
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetProductAsync(invalidId));
        }

        [Fact]
        public async Task GetProductAsync_ValidId_ReturnsProduct()
        {
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(42))
                .ReturnsAsync(new ProductDetailModel { InstanceId = 42, Name = "Test" });

            var service = new ProductService(mockRepo.Object);

            var result = await service.GetProductAsync(42);

            Assert.Equal(42, result.InstanceId);
        }

        #endregion

        #region SearchProductsAsync

        [Fact]
        public async Task SearchProductsAsync_ValidRequest_ReturnsResults()
        {
            var request = new ProductSearchRequest
            {
                Page = 1,
                PageSize = 25,
            };

            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.SearchAsync(request))
                .ReturnsAsync(new PaginatedResult<ProductModel>
                {
                    Items = new System.Collections.Generic.List<ProductModel>(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 25,
                });

            var service = new ProductService(mockRepo.Object);

            var result = await service.SearchProductsAsync(request);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task SearchProductsAsync_NullRequest_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.SearchProductsAsync(null!));
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        public async Task SearchProductsAsync_InvalidPage_ThrowsArgumentOutOfRange(int page, int pageSize)
        {
            var request = new ProductSearchRequest { Page = page, PageSize = pageSize };

            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.SearchProductsAsync(request));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public async Task SearchProductsAsync_InvalidPageSize_ThrowsArgumentOutOfRange(int page, int pageSize)
        {
            var request = new ProductSearchRequest { Page = page, PageSize = pageSize };

            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.SearchProductsAsync(request));
        }

        #endregion

        #region Constructor

        [Fact]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProductService(null!));
        }

        #endregion
    }
}