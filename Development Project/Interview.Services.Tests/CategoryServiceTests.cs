using System;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Interview.Services;
using Moq;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;
using System.Data;

namespace Interview.Services.Tests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ISqlExecutor> _mockSqlExecutor;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockSqlExecutor = new Mock<ISqlExecutor>();
            _categoryService = new CategoryService(_mockSqlExecutor.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateCategoryAsync_ShouldThrowArgumentException_WhenNameIsInvalid(string name)
        {
            // Arrange
            var request = new CategoryRequest
            {
                Name = name,
                Description = "Valid Description"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _categoryService.CreateCategoryAsync(request));
            Assert.Contains("Category name is required", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateCategoryAsync_ShouldThrowArgumentException_WhenDescriptionIsInvalid(string description)
        {
            // Arrange
            var request = new CategoryRequest
            {
                Name = "Valid Name",
                Description = description
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _categoryService.CreateCategoryAsync(request));
            Assert.Contains("Category description is required", exception.Message);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldCallSqlExecutor_WhenRequestIsValid()
        {
            // Arrange
            var request = new CategoryRequest
            {
                Name = "Electronics",
                Description = "Electronic goods"
            };

            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(1);

            // Act
            var result = await _categoryService.CreateCategoryAsync(request);

            // Assert
            Assert.Equal(1, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()), Times.Once);
        }

        [Fact]
        public async Task AddProductToCategoryAsync_ShouldCallSqlExecutor()
        {
            // Arrange
            int productId = 10;
            int categoryId = 20;

            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _categoryService.AddProductToCategoryAsync(productId, categoryId);

            // Assert
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()), Times.Once);
        }
    }
}
