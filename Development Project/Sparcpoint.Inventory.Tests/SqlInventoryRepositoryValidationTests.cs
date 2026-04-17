using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.SqlServer.Repositories;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class SqlInventoryRepositoryValidationTests
    {
        private static SqlInventoryRepository NewRepo(out Mock<ISqlExecutor> executor)
        {
            executor = new Mock<ISqlExecutor>();
            return new SqlInventoryRepository(executor.Object);
        }

        [Fact]
        public async Task RecordAsync_Throws_On_Null_List()
        {
            var repo = NewRepo(out _);
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.RecordAsync(null));
        }

        [Fact]
        public async Task RecordAsync_Returns_Empty_On_Empty_Input_Without_Hitting_Executor()
        {
            var repo = NewRepo(out var executor);
            var result = await repo.RecordAsync(new List<InventoryAdjustment>());
            Assert.Empty(result);
            executor.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RecordAsync_Throws_On_Zero_Quantity()
        {
            var repo = NewRepo(out _);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                repo.RecordAsync(new[] { new InventoryAdjustment(1, 0m, "ADD") }));
        }

        [Fact]
        public async Task RecordAsync_Throws_On_Invalid_Product_Id()
        {
            var repo = NewRepo(out _);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                repo.RecordAsync(new[] { new InventoryAdjustment(0, 5m) }));
        }

        [Fact]
        public async Task RecordAsync_Throws_When_TypeCategory_Too_Long()
        {
            var repo = NewRepo(out _);
            var bigCat = new string('X', 33);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                repo.RecordAsync(new[] { new InventoryAdjustment(1, 5m, bigCat) }));
        }

        [Fact]
        public async Task GetCountsAsync_Throws_On_Null_Query()
        {
            var repo = NewRepo(out _);
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.GetCountsAsync(null));
        }
    }
}
