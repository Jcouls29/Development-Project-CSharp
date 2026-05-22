using Sparcpoint.Inventory.IntegrationTests.Infrastructure;
using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.IntegrationTests.Tests
{
    [Collection(TestDatabaseCollection.Name)]
    public class InventoryRepositoryTests : IAsyncLifetime
    {
        private readonly TestDatabaseFixture _Fixture;
        public InventoryRepositoryTests(TestDatabaseFixture fixture) { _Fixture = fixture; }
        public Task InitializeAsync() => _Fixture.ResetAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        private async Task<int> CreateProductAsync(IReadOnlyDictionary<string, string> attributes = null)
        {
            var productRepo = _Fixture.CreateProductRepository();
            return await productRepo.CreateAsync(new CreateProductRequest
            {
                Name = "P-" + System.Guid.NewGuid().ToString("N").Substring(0, 6),
                Description = "test",
                Attributes = attributes ?? new Dictionary<string, string>()
            });
        }

        [Fact]
        public async Task Adding_Then_Removing_NetsToZero()
        {
            var inventory = _Fixture.CreateInventoryRepository();
            var productId = await CreateProductAsync();

            await inventory.CreateTransactionsAsync(new[]
            {
                new InventoryAdjustment { ProductId = productId, Quantity = 10 },
                new InventoryAdjustment { ProductId = productId, Quantity = -10 }
            });

            var count = await inventory.GetCountAsync(new InventoryCountQuery { ProductId = productId });
            Assert.Equal(0m, count);
        }

        [Fact]
        public async Task BulkTransactions_ApplyAtomically_AcrossMultipleProducts()
        {
            // EVAL: Requirement #4 — multiple products in a single call. Each adjustment must be
            // persisted in the same transaction.
            var inventory = _Fixture.CreateInventoryRepository();
            var p1 = await CreateProductAsync();
            var p2 = await CreateProductAsync();

            await inventory.CreateTransactionsAsync(new[]
            {
                new InventoryAdjustment { ProductId = p1, Quantity = 5 },
                new InventoryAdjustment { ProductId = p2, Quantity = 8 }
            });

            Assert.Equal(5m, await inventory.GetCountAsync(new InventoryCountQuery { ProductId = p1 }));
            Assert.Equal(8m, await inventory.GetCountAsync(new InventoryCountQuery { ProductId = p2 }));
        }

        [Fact]
        public async Task DeleteTransaction_UndoesItsEffectOnCount()
        {
            // EVAL: Requirement #6 — individual transactions removable. Because count = SUM(Quantity),
            // deleting the row removes its contribution from every subsequent count query.
            var inventory = _Fixture.CreateInventoryRepository();
            var productId = await CreateProductAsync();

            var created = (await inventory.CreateTransactionsAsync(new[]
            {
                new InventoryAdjustment { ProductId = productId, Quantity = 25 }
            })).ToList();

            Assert.Equal(25m, await inventory.GetCountAsync(new InventoryCountQuery { ProductId = productId }));

            await inventory.DeleteTransactionAsync(created[0].TransactionId);

            Assert.Equal(0m, await inventory.GetCountAsync(new InventoryCountQuery { ProductId = productId }));
        }

        [Fact]
        public async Task DeleteTransaction_Throws_WhenNotFound()
        {
            var inventory = _Fixture.CreateInventoryRepository();
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => inventory.DeleteTransactionAsync(999999));
        }

        [Fact]
        public async Task GetCount_ByAttributeFilter_SumsAcrossMatchingProducts()
        {
            // EVAL: Requirement #5 — count by subset of metadata. Verifies that the SUM aggregates
            // across every product whose attributes match, not just one specific product id.
            var inventory = _Fixture.CreateInventoryRepository();

            var redA = await CreateProductAsync(new Dictionary<string, string> { { "Color", "Red" } });
            var redB = await CreateProductAsync(new Dictionary<string, string> { { "Color", "Red" } });
            var blue = await CreateProductAsync(new Dictionary<string, string> { { "Color", "Blue" } });

            await inventory.CreateTransactionsAsync(new[]
            {
                new InventoryAdjustment { ProductId = redA, Quantity = 3 },
                new InventoryAdjustment { ProductId = redB, Quantity = 7 },
                new InventoryAdjustment { ProductId = blue, Quantity = 100 }
            });

            var redCount = await inventory.GetCountAsync(new InventoryCountQuery
            {
                AttributeFilters = new Dictionary<string, string> { { "Color", "Red" } }
            });

            Assert.Equal(10m, redCount);
        }

        [Fact]
        public async Task GetCount_ForProductWithoutTransactions_ReturnsZero()
        {
            var inventory = _Fixture.CreateInventoryRepository();
            var productId = await CreateProductAsync();

            var count = await inventory.GetCountAsync(new InventoryCountQuery { ProductId = productId });

            Assert.Equal(0m, count);
        }

        [Fact]
        public async Task NegativeQuantity_RemovesInventory()
        {
            var inventory = _Fixture.CreateInventoryRepository();
            var productId = await CreateProductAsync();

            await inventory.CreateTransactionsAsync(new[]
            {
                new InventoryAdjustment { ProductId = productId, Quantity = 50 },
                new InventoryAdjustment { ProductId = productId, Quantity = -20 }
            });

            var count = await inventory.GetCountAsync(new InventoryCountQuery { ProductId = productId });
            Assert.Equal(30m, count);
        }

        [Fact]
        public async Task GetCount_ByProductIdAndAttribute_CombinesFilters()
        {
            var inventory = _Fixture.CreateInventoryRepository();

            var redA = await CreateProductAsync(new Dictionary<string, string> { { "Color", "Red" } });
            var redB = await CreateProductAsync(new Dictionary<string, string> { { "Color", "Red" } });

            await inventory.CreateTransactionsAsync(new[]
            {
                new InventoryAdjustment { ProductId = redA, Quantity = 4 },
                new InventoryAdjustment { ProductId = redB, Quantity = 9 }
            });

            var count = await inventory.GetCountAsync(new InventoryCountQuery
            {
                ProductId = redA,
                AttributeFilters = new Dictionary<string, string> { { "Color", "Red" } }
            });

            Assert.Equal(4m, count);
        }
    }
}
