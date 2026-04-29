using Dapper;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to add must be positive.");

            return await _Executor.ExecuteAsync(async (conn, tx) =>
                await InsertTransactionAsync(conn, tx, productInstanceId, quantity, typeCategory));
        }

        public async Task AddBatchAsync(IEnumerable<InventoryBatchItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            // EVAL: All batch items share a single transaction — either all succeed or all roll back.
            await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                foreach (var item in items)
                {
                    if (item.Quantity <= 0)
                        throw new ArgumentOutOfRangeException(nameof(item.Quantity), "Batch add quantity must be positive.");
                    await InsertTransactionAsync(conn, tx, item.ProductInstanceId, item.Quantity, item.TypeCategory);
                }
            });
        }

        public async Task<int> RemoveAsync(int productInstanceId, decimal quantity, string typeCategory = null)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to remove must be positive.");

            // Store removals as negative quantities — net SUM gives the true on-hand count.
            return await _Executor.ExecuteAsync(async (conn, tx) =>
                await InsertTransactionAsync(conn, tx, productInstanceId, -quantity, typeCategory));
        }

        public async Task RemoveBatchAsync(IEnumerable<InventoryBatchItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                foreach (var item in items)
                {
                    if (item.Quantity <= 0)
                        throw new ArgumentOutOfRangeException(nameof(item.Quantity), "Batch remove quantity must be positive.");
                    await InsertTransactionAsync(conn, tx, item.ProductInstanceId, -item.Quantity, item.TypeCategory);
                }
            });
        }

        public async Task DeleteTransactionAsync(int transactionId)
        {
            await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                var affected = await conn.ExecuteAsync(
                    "DELETE FROM [Transactions].[InventoryTransactions] WHERE [TransactionId] = @TransactionId",
                    new { TransactionId = transactionId }, tx);

                if (affected == 0)
                    throw new InvalidOperationException($"Transaction {transactionId} not found.");
            });
        }

        public async Task<decimal> GetCountAsync(int productInstanceId)
        {
            // EVAL: CompletedTimestamp IS NOT NULL is the filter for "committed" transactions.
            // All transactions inserted by this repository set CompletedTimestamp = SYSUTCDATETIME() immediately,
            // so the filter is effectively "all transactions". The column is preserved to support a future
            // "pending transaction" workflow where a transaction is created but not yet confirmed.
            return await _Executor.ExecuteAsync(async (conn, tx) =>
                await conn.ExecuteScalarAsync<decimal>(@"
                    SELECT ISNULL(SUM([Quantity]), 0)
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [ProductInstanceId] = @ProductInstanceId
                      AND [CompletedTimestamp] IS NOT NULL",
                    new { ProductInstanceId = productInstanceId }, tx));
        }

        public async Task<IEnumerable<ProductInventoryCount>> GetCountByFilterAsync(ProductSearchFilter filter)
        {
            return await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                var sql = new StringBuilder(@"
                    SELECT p.[InstanceId]              AS ProductInstanceId,
                           p.[Name]                   AS ProductName,
                           ISNULL(SUM(t.[Quantity]), 0) AS TotalQuantity
                    FROM [Instances].[Products] p
                    LEFT JOIN [Transactions].[InventoryTransactions] t
                           ON t.[ProductInstanceId] = p.[InstanceId]
                          AND t.[CompletedTimestamp] IS NOT NULL
                    WHERE 1=1");

                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(filter?.Name))
                {
                    sql.Append(" AND p.[Name] LIKE @Name");
                    parameters.Add("Name", $"%{filter.Name}%");
                }

                if (filter?.Attributes != null)
                {
                    int idx = 0;
                    foreach (var attr in filter.Attributes)
                    {
                        var k = $"AttrKey{idx}";
                        var v = $"AttrVal{idx}";
                        sql.Append($@" AND EXISTS (
                            SELECT 1 FROM [Instances].[ProductAttributes] pa{idx}
                            WHERE pa{idx}.[InstanceId] = p.[InstanceId]
                              AND pa{idx}.[Key] = @{k} AND pa{idx}.[Value] = @{v})");
                        parameters.Add(k, attr.Key);
                        parameters.Add(v, attr.Value);
                        idx++;
                    }
                }

                if (filter?.CategoryIds != null && filter.CategoryIds.Any())
                {
                    sql.Append(@" AND EXISTS (
                        SELECT 1 FROM [Instances].[ProductCategories] pc
                        WHERE pc.[InstanceId] = p.[InstanceId]
                          AND pc.[CategoryInstanceId] IN @CategoryIds)");
                    parameters.Add("CategoryIds", filter.CategoryIds);
                }

                sql.Append(" GROUP BY p.[InstanceId], p.[Name] ORDER BY p.[Name]");

                return await conn.QueryAsync<ProductInventoryCount>(sql.ToString(), parameters, tx);
            });
        }

        private static async Task<int> InsertTransactionAsync(
            IDbConnection conn, IDbTransaction tx,
            int productInstanceId, decimal quantity, string typeCategory)
        {
            // EVAL: Guard against FK violation (FK_InventoryTransactions_Products) by checking
            // product existence within the same transaction before the INSERT. This produces
            // a clear 400 instead of letting SQL Server throw a 547 FK error as a 500.
            // Called by AddAsync, RemoveAsync, AddBatchAsync, and RemoveBatchAsync — one check covers all paths.
            var productExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM [Instances].[Products] WHERE [InstanceId] = @Id",
                new { Id = productInstanceId }, tx);

            if (productExists == 0)
                throw new ArgumentException(
                    $"Product with InstanceId {productInstanceId} does not exist.",
                    nameof(productInstanceId));

            return await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO [Transactions].[InventoryTransactions]
                    ([ProductInstanceId], [Quantity], [TypeCategory], [CompletedTimestamp])
                VALUES (@ProductInstanceId, @Quantity, @TypeCategory, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);",
                new { ProductInstanceId = productInstanceId, Quantity = quantity, TypeCategory = typeCategory },
                tx);
        }
    }
}
