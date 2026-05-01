using Moq;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.Inventory.Models.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class ProductRepositoryTests
    {
        private readonly Mock<ISqlExecutor> _MockExecutor;
        private readonly SqlProductRepository _Repository;

        public ProductRepositoryTests()
        {
            _MockExecutor = new Mock<ISqlExecutor>();
            _Repository = new SqlProductRepository(_MockExecutor.Object);
        }

        [Fact]
        public void Constructor_NullExecutor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlProductRepository(null));
        }

        [Fact]
        public async Task AddAsync_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.AddAsync(null));
        }

        [Fact]
        public async Task AddAsync_EmptyName_ThrowsArgumentException()
        {
            var request = new AddProductRequest { Name = "", Description = "A description" };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_WhitespaceName_ThrowsArgumentException()
        {
            var request = new AddProductRequest { Name = "   ", Description = "A description" };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_EmptyDescription_ThrowsArgumentException()
        {
            var request = new AddProductRequest { Name = "Widget", Description = "" };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_ValidRequest_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<int>(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(7);

            var request = new AddProductRequest { Name = "Widget", Description = "A widget" };
            var result = await _Repository.AddAsync(request);

            Assert.Equal(7, result);
            _MockExecutor.Verify(e => e.ExecuteAsync<int>(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.SearchAsync(null));
        }

        [Fact]
        public async Task SearchAsync_EmptyRequest_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<System.Collections.Generic.IEnumerable<Models.Product>>(
                    It.IsAny<Func<IDbConnection, IDbTransaction, Task<System.Collections.Generic.IEnumerable<Models.Product>>>>()))
                .ReturnsAsync(Array.Empty<Models.Product>());

            var result = await _Repository.SearchAsync(new ProductSearchRequest());

            Assert.NotNull(result);
        }
    }
}
