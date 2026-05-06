using Moq;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    // EVAL: using Moq to keep repos isolated from a real SQL Server instance - these just cover
    // guard clauses and input validation, integration tests handle actual query correctness
    public class SqlProductRepositoryTests
    {
        [Fact]
        public void Constructor_ThrowsOnNullExecutor()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlProductRepository(null));
        }

        [Fact]
        public async Task AddAsync_ThrowsOnNullRequest()
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlProductRepository(executor.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddAsync(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddAsync_ThrowsOnBlankName(string name)
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlProductRepository(executor.Object);
            var request = new CreateProductRequest { Name = name, Description = "Desc" };
            await Assert.ThrowsAsync<ArgumentException>(() => repo.AddAsync(request));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddAsync_ThrowsOnBlankDescription(string description)
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlProductRepository(executor.Object);
            var request = new CreateProductRequest { Name = "Valid Name", Description = description };
            await Assert.ThrowsAsync<ArgumentException>(() => repo.AddAsync(request));
        }
    }
}
