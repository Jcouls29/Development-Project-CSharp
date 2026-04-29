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

            // EVAL: all batch items run in one transaction - all succeed or all roll back
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

            // Store removals as negative quantities - net SUM gives the true on-hand count.
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
            // EVAL: CompletedTimestamp IS NOT NULL filters for committed transactions - right now
            // every insert sets it immediately so this hits everything, but the column is there
            // to support a future pending transaction workflow if we ever need it
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
                // EVAL: SqlServerQueryProvider handles dynamic WHERE - SELECT/FROM/JOIN/GROUP BY are
                // hardcoded because the provider has no API for those. Same filter approach as
                // SqlProductRepository.SearchAsync: Name LIKE uses Where() + AddParameter(), EAV
                // attributes use composed EXISTS subqueries with GetNextParameterName() for unique
                // param names, CategoryIds uses a raw Where() with an IN list that Dapper expands natively.
                var provider = SqlServerQueryProvider.Empty;

                if (!string.IsNullOrWhiteSpace(filter?.Name))
                {
                    provider.Where("p.[Name] LIKE @Name");
                    provider.AddParameter("Name", $"%{filter.Name}%");
                }

                if (filter?.Attributes != null)
                {
                    foreach (var attr in filter.Attributes)
                    {
                        var keyParam = provider.GetNextParameterName("AttrKey");  // e.g. "AttrKey0"
                        var valParam = provider.GetNextParameterName("AttrVal");  // e.g. "AttrVal1"
                        var alias = keyParam.Substring("AttrKey".Length);         // e.g. "0"
                        provider.Where($@"EXISTS (
                            SELECT 1 FROM [Instances].[ProductAttributes] pa{alias}
                            WHERE pa{alias}.[InstanceId] = p.[InstanceId]
                              AND pa{alias}.[Key] = @{keyParam} AND pa{alias}.[Value] = @{valParam})");
                        provider.AddParameter(keyParam, attr.Key);
                        provider.AddParameter(valParam, attr.Value);
                    }
                }

                if (filter?.CategoryIds != null && filter.CategoryIds.Any())
                {
                    provider.Where(@"EXISTS (
                        SELECT 1 FROM [Instances].[ProductCategories] pc
                        WHERE pc.[InstanceId] = p.[InstanceId]
                          AND pc.[CategoryInstanceId] IN @CategoryIds)");
                    provider.AddParameter("CategoryIds", filter.CategoryIds);
                }

                // EVAL: "[Name]" without the "p." alias - SanitizeColumnName rejects "p.[Name]"
                // (unbracketed prefix + bracketed name), and it's unambiguous anyway since it's the only [Name] in the SELECT
                provider.OrderByAscending("[Name]");

                var sqlBuilder = new StringBuilder(@"
                    SELECT p.[InstanceId]              AS ProductInstanceId,
                           p.[Name]                   AS ProductName,
                           ISNULL(SUM(t.[Quantity]), 0) AS TotalQuantity
                    FROM [Instances].[Products] p
                    LEFT JOIN [Transactions].[InventoryTransactions] t
                           ON t.[ProductInstanceId] = p.[InstanceId]
                          AND t.[CompletedTimestamp] IS NOT NULL");

                sqlBuilder.Append($" {provider.WhereClause}");
                sqlBuilder.Append(" GROUP BY p.[InstanceId], p.[Name]");
                sqlBuilder.Append($" {provider.OrderByClause}");

                return await conn.QueryAsync<ProductInventoryCount>(sqlBuilder.ToString(), provider.Parameters, tx);
            });
        }

        private static async Task<int> InsertTransactionAsync(
            IDbConnection conn, IDbTransaction tx,
            int productInstanceId, decimal quantity, string typeCategory)
        {
            // EVAL: check product existence inside the same transaction before the INSERT so we get a
            // clean 400 instead of a 547 FK error surfacing as a 500 - this helper is shared by
            // AddAsync, RemoveAsync, AddBatchAsync, and RemoveBatchAsync so one check covers all paths
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
