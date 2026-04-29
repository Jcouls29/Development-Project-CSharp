using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using Xunit;

// EVAL: SparcpointInventory must exist on (localdb)\MSSQLLocalDB with the full schema applied
// before running these, use the SQL scripts in the project. Without it you'll get
// "Invalid object name" errors right away, these tests don't self-bootstrap.

namespace Sparcpoint.Inventory.Tests
{
    // EVAL: using TransactionScope instead of TRUNCATE/DELETE between tests for a few reasons:
    // rollback is one log operation vs truncating tables with FK constraints which needs a specific
    // deletion order, TRUNCATE also needs ALTER TABLE perms we don't have, and if a test crashes
    // mid-run scope.Dispose() without Complete() auto-rolls back so no orphaned data on the dev DB.
    // Works with SqlServerExecutor because Microsoft.Data.SqlClient auto-enlists new connections in
    // an ambient TransactionScope - conn.BeginTransaction() gets a nested transaction that
    // participates in the scope, so disposing without Complete() rolls back everything.
    [Trait("Category", "Integration")]
    public class IntegrationTests
    {
        private const string ConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=SparcpointInventory;Trusted_Connection=True;TrustServerCertificate=True;";

        // -----------------------------------------------------------------------------------------
        // Test 1: Create a product with a known attribute, then verify SearchAsync finds it
        // -----------------------------------------------------------------------------------------
        [Fact]
        public async Task AddProduct_ThenSearch_ByAttribute_ReturnsProduct()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var executor = new SqlServerExecutor(ConnectionString);
            var productRepo = new SqlProductRepository(executor);

            var request = new CreateProductRequest
            {
                Name        = "Integration Test Widget",
                Description = "Test",
                Attributes  = new Dictionary<string, string>
                {
                    { "Brand", "ACME" }
                }
            };

            await productRepo.AddAsync(request);

            var results = await productRepo.SearchAsync(new ProductSearchFilter
            {
                Attributes = new Dictionary<string, string>
                {
                    { "Brand", "ACME" }
                }
            });

            Assert.Contains(results, p => p.Name == "Integration Test Widget");

            // No scope.Complete() - automatic rollback on dispose keeps the database clean.
        }

        // -----------------------------------------------------------------------------------------
        // Test 2: Add inventory for a product and verify GetCountAsync returns the correct quantity
        // -----------------------------------------------------------------------------------------
        [Fact]
        public async Task AddInventory_ThenGetCount_ReturnsCorrectQuantity()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var executor = new SqlServerExecutor(ConnectionString);
            var productRepo   = new SqlProductRepository(executor);
            var inventoryRepo = new SqlInventoryRepository(executor);

            // A valid product is required to satisfy the FK on InventoryTransactions.
            var productId = await productRepo.AddAsync(new CreateProductRequest
            {
                Name        = "Inventory Count Test Product",
                Description = "Test"
            });

            await inventoryRepo.AddAsync(productId, 50m);

            var count = await inventoryRepo.GetCountAsync(productId);

            Assert.Equal(50m, count);

            // No scope.Complete() - automatic rollback.
        }

        // -----------------------------------------------------------------------------------------
        // Test 3: Delete a transaction and verify the inventory count returns to zero
        // -----------------------------------------------------------------------------------------
        [Fact]
        public async Task DeleteTransaction_UndoesInventoryEffect()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var executor = new SqlServerExecutor(ConnectionString);
            var productRepo   = new SqlProductRepository(executor);
            var inventoryRepo = new SqlInventoryRepository(executor);

            var productId = await productRepo.AddAsync(new CreateProductRequest
            {
                Name        = "Delete Transaction Test Product",
                Description = "Test"
            });

            var transactionId = await inventoryRepo.AddAsync(productId, 100m);

            await inventoryRepo.DeleteTransactionAsync(transactionId);

            var count = await inventoryRepo.GetCountAsync(productId);

            Assert.Equal(0m, count);

            // No scope.Complete() - automatic rollback.
        }
    }
}
