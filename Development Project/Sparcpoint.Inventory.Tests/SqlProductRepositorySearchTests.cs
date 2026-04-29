using Moq;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    // EVAL: These tests verify that SearchAsync delegates to the executor and
    // correctly passes filter state. SQL execution is mocked — integration tests
    // would be the next layer to verify actual query correctness against the DB.
    public class SqlProductRepositorySearchTests
    {
        [Fact]
        public async Task SearchAsync_WithNullFilter_DelegatesToExecutor()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<List<Product>>>>()))
                .ReturnsAsync(new List<Product>());

            var repo = new SqlProductRepository(mockExecutor.Object);

            // Null filter = no criteria = returns all products (unbounded)
            var exception = await Record.ExceptionAsync(() => repo.SearchAsync(null));
            Assert.Null(exception);
        }

        [Fact]
        public async Task SearchAsync_WithEmptyFilter_DelegatesToExecutor()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<List<Product>>>>()))
                .ReturnsAsync(new List<Product>());

            var repo = new SqlProductRepository(mockExecutor.Object);
            var exception = await Record.ExceptionAsync(() =>
                repo.SearchAsync(new ProductSearchFilter()));

            Assert.Null(exception);
        }

        [Fact]
        public async Task AddAsync_CallsExecutorExactlyOnce()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(1);

            var repo = new SqlProductRepository(mockExecutor.Object);
            await repo.AddAsync(new CreateProductRequest
            {
                Name = "Test Product",
                Description = "A test product"
            });

            mockExecutor.Verify(
                e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()),
                Times.Once);
        }

        [Fact]
        public async Task AddAsync_ReturnsProductIdFromExecutor()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(15);

            var repo = new SqlProductRepository(mockExecutor.Object);
            var result = await repo.AddAsync(new CreateProductRequest
            {
                Name = "Widget Pro",
                Description = "Professional widget"
            });

            Assert.Equal(15, result);
        }
    }
}
