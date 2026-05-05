// EVAL: Inventory operations use an append-only transaction log pattern.
// Adding inventory = positive quantity row, removing = negative quantity row.
// Undo = set CompletedTimestamp (soft-delete). Current count = SUM of active rows.
// This design provides full audit trail and reversibility.

using Dapper;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    /// <summary>
    /// SQL Server implementation of <see cref="IInventoryRepository"/>.
    /// </summary>
    public class SqlServerInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlServerInventoryRepository(ISqlExecutor executor)
        {
            PreConditions.ParameterNotNull(executor, nameof(executor));
            _executor = executor;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InventoryTransaction>> AddInventoryAsync(
            IEnumerable<InventoryTransactionItem> items,
            CancellationToken cancellationToken = default)
        {
            // EVAL: Positive quantity = inventory added
            return await InsertTransactionsAsync(items, negate: false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InventoryTransaction>> RemoveInventoryAsync(
            IEnumerable<InventoryTransactionItem> items,
            CancellationToken cancellationToken = default)
        {
            // EVAL: Negate quantity so removal is stored as negative in the DB.
            // This keeps the count calculation simple: SUM(Quantity).
            return await InsertTransactionsAsync(items, negate: true);
        }

        /// <inheritdoc />
        public async Task<decimal?> GetCountByProductIdAsync(
            int productInstanceId,
            CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteAsync<decimal?>(async (conn, trans) =>
            {
                // RV: First verify the product exists to distinguish "not found" from "zero count"
                const string existsSql = @"
                    SELECT COUNT(1) FROM [Instances].[Products]
                    WHERE [InstanceId] = @ProductInstanceId;";

                var exists = await conn.QuerySingleAsync<int>(existsSql, new { ProductInstanceId = productInstanceId }, trans);
                if (exists == 0)
                    return null;

                // EVAL: SUM of Quantity where CompletedTimestamp IS NULL gives active inventory count.
                // ISNULL handles the case where no transactions exist (returns 0, not NULL).
                const string countSql = @"
                    SELECT ISNULL(SUM([Quantity]), 0)
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [ProductInstanceId] = @ProductInstanceId
                      AND [CompletedTimestamp] IS NULL;";

                return await conn.QuerySingleAsync<decimal>(
                    countSql,
                    new { ProductInstanceId = productInstanceId },
                    trans);
            });
        }

        /// <inheritdoc />
        public async Task<Dictionary<int, decimal>> GetCountByMetadataAsync(
            string attributeKey,
            string attributeValue,
            CancellationToken cancellationToken = default)
        {
            PreConditions.StringNotNullOrWhitespace(attributeKey, nameof(attributeKey));
            PreConditions.StringNotNullOrWhitespace(attributeValue, nameof(attributeValue));

            return await _executor.ExecuteAsync<Dictionary<int, decimal>>(async (conn, trans) =>
            {
                // EVAL: JOIN through ProductAttributes to find products matching the metadata,
                // then SUM their active inventory transactions.
                // This fulfills Requirement 5: count by metadata subset.
                const string sql = @"
                    SELECT it.[ProductInstanceId],
                           ISNULL(SUM(it.[Quantity]), 0) AS [Count]
                    FROM [Transactions].[InventoryTransactions] it
                    INNER JOIN [Instances].[ProductAttributes] pa
                        ON it.[ProductInstanceId] = pa.[InstanceId]
                    WHERE pa.[Key] = @Key
                      AND pa.[Value] = @Value
                      AND it.[CompletedTimestamp] IS NULL
                    GROUP BY it.[ProductInstanceId];";

                var rows = await conn.QueryAsync<CountRow>(
                    sql,
                    new { Key = attributeKey, Value = attributeValue },
                    trans);

                return rows.ToDictionary(r => r.ProductInstanceId, r => r.Count);
            });
        }

        /// <inheritdoc />
        public async Task<InventoryTransaction> UndoTransactionAsync(
            int transactionId,
            CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteAsync<InventoryTransaction>(async (conn, trans) =>
            {
                // 1. Find the transaction
                const string selectSql = @"
                    SELECT [TransactionId], [ProductInstanceId], [Quantity],
                           [StartedTimestamp], [CompletedTimestamp], [TypeCategory]
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [TransactionId] = @TransactionId;";

                var transaction = await conn.QuerySingleOrDefaultAsync<InventoryTransaction>(
                    selectSql,
                    new { TransactionId = transactionId },
                    trans);

                if (transaction == null)
                    return null;

                // RV: If already undone, return the existing record.
                // The controller will check IsActive and return 409 Conflict.
                if (!transaction.IsActive)
                    return transaction;

                // 2. Set CompletedTimestamp to mark as undone
                // EVAL: Soft-delete preserves the transaction for audit purposes.
                // The row is excluded from inventory counts because CompletedTimestamp IS NOT NULL.
                const string updateSql = @"
                    UPDATE [Transactions].[InventoryTransactions]
                    SET [CompletedTimestamp] = SYSUTCDATETIME()
                    WHERE [TransactionId] = @TransactionId;

                    SELECT [TransactionId], [ProductInstanceId], [Quantity],
                           [StartedTimestamp], [CompletedTimestamp], [TypeCategory]
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [TransactionId] = @TransactionId;";

                return await conn.QuerySingleAsync<InventoryTransaction>(
                    updateSql,
                    new { TransactionId = transactionId },
                    trans);
            });
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync(
            int? productInstanceId,
            bool activeOnly,
            CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteAsync<IEnumerable<InventoryTransaction>>(async (conn, trans) =>
            {
                var sql = @"
                    SELECT [TransactionId], [ProductInstanceId], [Quantity],
                           [StartedTimestamp], [CompletedTimestamp], [TypeCategory]
                    FROM [Transactions].[InventoryTransactions]
                    WHERE 1 = 1";

                if (productInstanceId.HasValue)
                    sql += " AND [ProductInstanceId] = @ProductInstanceId";

                if (activeOnly)
                    sql += " AND [CompletedTimestamp] IS NULL";

                sql += " ORDER BY [StartedTimestamp] DESC, [TransactionId] DESC;";

                return await conn.QueryAsync<InventoryTransaction>(
                    sql,
                    new { ProductInstanceId = productInstanceId },
                    trans);
            });
        }

        #region Private Helpers

        /// <summary>
        /// Inserts one or more inventory transactions within a single DB transaction.
        /// </summary>
        /// <param name="items">Items to insert.</param>
        /// <param name="negate">If true, quantities are stored as negative (for removals).</param>
        private async Task<IEnumerable<InventoryTransaction>> InsertTransactionsAsync(
            IEnumerable<InventoryTransactionItem> items,
            bool negate)
        {
            return await _executor.ExecuteAsync<IEnumerable<InventoryTransaction>>(async (conn, trans) =>
            {
                var results = new List<InventoryTransaction>();

                // EVAL: Each item in the batch is inserted within the same transaction.
                // If any insert fails (e.g., invalid FK), the entire batch rolls back.
                // TVPs were considered but require custom SqlMapper.ICustomQueryParameter
                // implementations with Dapper, and the per-row INSERT approach is clearer
                // and sufficient for typical API batch sizes. The individual INSERT also
                // gives us SCOPE_IDENTITY() per row for returning transaction IDs.
                const string insertSql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [TypeCategory])
                    VALUES
                        (@ProductInstanceId, @Quantity, @TypeCategory);

                    SELECT [TransactionId], [ProductInstanceId], [Quantity],
                           [StartedTimestamp], [CompletedTimestamp], [TypeCategory]
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [TransactionId] = SCOPE_IDENTITY();";

                foreach (var item in items)
                {
                    var quantity = negate ? -item.Quantity : item.Quantity;

                    var transaction = await conn.QuerySingleAsync<InventoryTransaction>(
                        insertSql,
                        new
                        {
                            item.ProductInstanceId,
                            Quantity = quantity,
                            item.TypeCategory
                        },
                        trans);

                    results.Add(transaction);
                }

                return results;
            });
        }

        #endregion

        #region Internal Row Types

        private class CountRow
        {
            public int ProductInstanceId { get; set; }
            public decimal Count { get; set; }
        }

        #endregion
    }
}
