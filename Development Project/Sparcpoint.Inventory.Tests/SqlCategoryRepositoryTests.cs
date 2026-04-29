using Moq;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class SqlCategoryRepositoryTests
    {
        [Fact]
        public void Constructor_ThrowsOnNullExecutor()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlCategoryRepository(null));
        }

        [Fact]
        public async Task AddAsync_ThrowsOnNullRequest()
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlCategoryRepository(executor.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddAsync(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddAsync_ThrowsOnBlankName(string name)
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlCategoryRepository(executor.Object);
            var request = new CreateCategoryRequest { Name = name, Description = "Desc" };
            await Assert.ThrowsAsync<ArgumentException>(() => repo.AddAsync(request));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddAsync_ThrowsOnBlankDescription(string description)
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlCategoryRepository(executor.Object);
            var request = new CreateCategoryRequest { Name = "Electronics", Description = description };
            await Assert.ThrowsAsync<ArgumentException>(() => repo.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_DelegatesToExecutorAndReturnsCategoryId()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(7);

            var repo = new SqlCategoryRepository(mockExecutor.Object);
            var result = await repo.AddAsync(new CreateCategoryRequest
            {
                Name = "Electronics",
                Description = "Electronic devices"
            });

            Assert.Equal(7, result);
            mockExecutor.Verify(
                e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DelegatesToExecutor()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            // EVAL: The setup type must match the inferred T in ExecuteAsync<T>.
            // GetAllAsync calls .ToList() internally, so T = List<Category>, not IEnumerable<Category>.
            // Using IEnumerable<Category> causes Moq to miss the setup and return null.
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<System.Collections.Generic.List<Category>>>>()))
                .ReturnsAsync(new System.Collections.Generic.List<Category>());

            var repo = new SqlCategoryRepository(mockExecutor.Object);
            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
        }
    }
}
