using System;
using System.Data;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories;
using Interview.Web.Repositories.Interfaces;
using Moq;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Interview.Tests
{
    public class ProductRepositoryTests
    {
        private readonly Mock<ISqlExecutor> _mockSqlExecutor;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            _mockSqlExecutor = new Mock<ISqlExecutor>();
            _repository = new ProductRepository(_mockSqlExecutor.Object);
        }

        [Fact]
        public async Task AddProductAsync_CallsExecuteAsync()
        {
            // Arrange
            var request = new CreateProductRequest { Name = "Test" };
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                            .ReturnsAsync(1);

            // Act
            var result = await _repository.AddProductAsync(request);

            // Assert
            Assert.Equal(1, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()), Times.Once);
        }
    }
}
