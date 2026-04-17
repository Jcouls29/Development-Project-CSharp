using System;
using System.Threading.Tasks;
using Moq;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.SqlServer.Repositories;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    /// <summary>
    /// EVAL: Contract validation tests — do not touch the real DB. We use
    /// Moq on ISqlExecutor to isolate the repository guard logic.
    /// </summary>
    public class SqlProductRepositoryValidationTests
    {
        private static SqlProductRepository NewRepo()
        {
            var executor = new Mock<ISqlExecutor>();
            // The executor should never be invoked in the guard tests — if it were,
            // it means the validation did not cut off the flow. If it does, verify.
            return new SqlProductRepository(executor.Object);
        }

        [Fact]
        public void Ctor_Throws_On_Null_Executor()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlProductRepository(null));
        }

        [Fact]
        public async Task AddAsync_Throws_On_Null_Product()
        {
            var repo = NewRepo();
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddAsync(null));
        }

        [Fact]
        public async Task AddAsync_Throws_On_Empty_Name()
        {
            var repo = NewRepo();
            await Assert.ThrowsAsync<ArgumentException>(() =>
                repo.AddAsync(new Product { Name = "   " }));
        }

        [Fact]
        public async Task AddAsync_Throws_On_Name_Too_Long()
        {
            var repo = NewRepo();
            var name = new string('x', 257);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                repo.AddAsync(new Product { Name = name }));
        }

        [Fact]
        public async Task AddAsync_Does_Not_Invoke_Executor_When_Validation_Fails()
        {
            var executor = new Mock<ISqlExecutor>(MockBehavior.Strict);
            var repo = new SqlProductRepository(executor.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddAsync(null));

            executor.VerifyNoOtherCalls();
        }
    }
}
