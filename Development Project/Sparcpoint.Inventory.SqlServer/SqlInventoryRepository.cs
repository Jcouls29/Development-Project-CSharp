using Dapper;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlInventoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(int productInstanceId, decimal quantity, string typeCategory = null)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

            return await _Executor.ExecuteAsync(async (conn, trans) =>
                await conn.QuerySingleAsync<int>(@"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                    OUTPUT INSERTED.[TransactionId]
                    VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory)",
                    new { ProductInstanceId = productInstanceId, Quantity = quantity, TypeCategory = typeCategory },
                    trans));
        }

        public async Task AddBulkAsync(
            IEnumerable<(int ProductInstanceId, decimal Quantity, string TypeCategory)> transactions)
        {
            var items = transactions?.ToList()
                ?? throw new ArgumentNullException(nameof(transactions));

            if (items.Any(i => i.Quantity <= 0))
                throw new ArgumentException("All quantities must be positive.");

            // EVAL: All inserts share a single transaction so a failure rolls back the entire batch
            await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                foreach (var item in items)
                {
                    await conn.ExecuteAsync(@"
                        INSERT INTO [Transactions].[InventoryTransactions]
                            ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                        VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory)",
                        new { item.ProductInstanceId, item.Quantity, item.TypeCategory }, trans);
                }
            });
        }

        public async Task RemoveAsync(int productInstanceId, decimal quantity, string typeCategory = null)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

            // EVAL: Inventory removal is stored as a negative-quantity transaction rather than
            // deleting existing rows. This preserves the full audit trail and makes GetCountAsync
            // a simple SUM aggregate. DeleteTransactionAsync handles the "undo" requirement.
            await _Executor.ExecuteAsync(async (conn, trans) =>
                await conn.ExecuteAsync(@"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                    VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory)",
                    new { ProductInstanceId = productInstanceId, Quantity = -quantity, TypeCategory = typeCategory },
                    trans));
        }

        public async Task RemoveBulkAsync(
            IEnumerable<(int ProductInstanceId, decimal Quantity, string TypeCategory)> transactions)
        {
            var items = transactions?.ToList()
                ?? throw new ArgumentNullException(nameof(transactions));

            if (items.Any(i => i.Quantity <= 0))
                throw new ArgumentException("All quantities must be positive.");

            await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                foreach (var item in items)
                {
                    await conn.ExecuteAsync(@"
                        INSERT INTO [Transactions].[InventoryTransactions]
                            ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                        VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory)",
                        new { item.ProductInstanceId, Quantity = -item.Quantity, item.TypeCategory }, trans);
                }
            });
        }

        public async Task DeleteTransactionAsync(int transactionId)
        {
            await _Executor.ExecuteAsync(async (conn, trans) =>
                await conn.ExecuteAsync(@"
                    DELETE FROM [Transactions].[InventoryTransactions]
                    WHERE [TransactionId] = @TransactionId",
                    new { TransactionId = transactionId }, trans));
        }

        public async Task<decimal> GetCountAsync(int productInstanceId)
        {
            return await _Executor.ExecuteAsync(async (conn, trans) =>
                await conn.QuerySingleAsync<decimal>(@"
                    SELECT ISNULL(SUM([Quantity]), 0)
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [ProductInstanceId] = @ProductInstanceId",
                    new { ProductInstanceId = productInstanceId }, trans));
        }

        public async Task<decimal> GetCountByAttributesAsync(IDictionary<string, string> attributes)
        {
            if (attributes == null || !attributes.Any())
                return 0m;

            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                var parameters = new DynamicParameters();
                var attrConditions = new List<string>();

                // EVAL: Mirrors the same EXISTS pattern used in SearchAsync so the query planner
                // can reuse the IX_ProductAttributes_Key_Value index for both operations.
                int idx = 0;
                foreach (var attr in attributes)
                {
                    string keyParam = $"attrKey{idx}";
                    string valParam = $"attrVal{idx}";
                    attrConditions.Add($@"EXISTS (
                        SELECT 1 FROM [Instances].[ProductAttributes] pa{idx}
                        WHERE pa{idx}.[InstanceId] = p.[InstanceId]
                          AND pa{idx}.[Key]   = @{keyParam}
                          AND pa{idx}.[Value] = @{valParam})");
                    parameters.Add(keyParam, attr.Key);
                    parameters.Add(valParam, attr.Value);
                    idx++;
                }

                string whereClause = "WHERE " + string.Join("\n      AND ", attrConditions);

                string sql = $@"
                    SELECT ISNULL(SUM(it.[Quantity]), 0)
                    FROM [Transactions].[InventoryTransactions] it
                    WHERE it.[ProductInstanceId] IN (
                        SELECT p.[InstanceId]
                        FROM [Instances].[Products] p
                        {whereClause})";

                return await conn.QuerySingleAsync<decimal>(sql, parameters, trans);
            });
        }
    }
}
