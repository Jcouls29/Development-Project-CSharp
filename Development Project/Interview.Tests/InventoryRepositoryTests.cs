using System;
using System.Data;
using System.Threading.Tasks;
using Interview.Web.Repositories;
using Interview.Web.Repositories.Interfaces;
using Moq;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Interview.Tests
{
    public class InventoryRepositoryTests
    {
        private readonly Mock<ISqlExecutor> _mockSqlExecutor;
        private readonly InventoryRepository _repository;

        public InventoryRepositoryTests()
        {
            _mockSqlExecutor = new Mock<ISqlExecutor>();
            _repository = new InventoryRepository(_mockSqlExecutor.Object);
        }

        [Fact]
        public async Task GetStockAsync_CallsExecuteAsync()
        {
            // Arrange
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                            .ReturnsAsync(100m);

            // Act
            var result = await _repository.GetStockAsync(1);

            // Assert
            Assert.Equal(100m, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()), Times.Once);
        }
    }
}
