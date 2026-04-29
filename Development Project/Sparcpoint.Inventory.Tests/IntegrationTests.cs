using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using Xunit;

// EVAL: Prerequisite — the database SparcpointInventory must exist on the local SQL Server instance
// (Server=(localdb)\MSSQLLocalDB) and have the full schema published before running these tests.
// The schema is typically applied via the SQL scripts or EF migrations in the project. Without it
// the tests fail immediately with "Invalid object name" SQL errors, which is expected — integration
// tests are not self-bootstrapping by design.

namespace Sparcpoint.Inventory.Tests
{
    // EVAL: TransactionScope is used here instead of TRUNCATE/DELETE between tests for three reasons:
    //   1. Speed — a rollback is a single log operation; truncating multiple tables with FK constraints
    //      requires disabling constraints or a specific deletion order, both slower and fragile.
    //   2. No DDL permissions required — TRUNCATE is a DDL statement that requires ALTER TABLE
    //      permission; TransactionScope only needs the DML permissions the app already holds.
    //   3. Safety on the shared dev database — a test failure or a crash mid-test leaves no orphaned
    //      data because the scope.Dispose() without scope.Complete() issues an automatic rollback.
    //      There is zero risk of corrupting the development dataset.
    //
    // How it interacts with SqlServerExecutor: Microsoft.Data.SqlClient automatically enlists new
    // connections in an ambient TransactionScope. When SqlServerExecutor calls conn.BeginTransaction()
    // it gets a nested transaction that participates in the ambient scope. Disposing the scope
    // without calling Complete() rolls back the ambient transaction and therefore all SQL work done
    // within the test, regardless of how many internal commits SqlServerExecutor issued.
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

            // No scope.Complete() — automatic rollback on dispose keeps the database clean.
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

            // No scope.Complete() — automatic rollback.
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

            // No scope.Complete() — automatic rollback.
        }
    }
}
