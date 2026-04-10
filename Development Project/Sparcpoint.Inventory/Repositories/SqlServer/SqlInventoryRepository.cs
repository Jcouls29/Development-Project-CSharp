using Dapper;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Data;

namespace Sparcpoint.Inventory.Repositories.SqlServer
{
    /// <summary>
    /// EVAL: SQL Server implementation of IInventoryRepository.
    /// Inventory is tracked via transactions — adding inventory creates a positive-quantity transaction,
    /// removing inventory creates a negative-quantity transaction.
    /// The "undo" feature uses soft delete (sets CompletedTimestamp = NULL) rather than hard delete,
    /// preserving the full audit trail. Since count queries filter on CompletedTimestamp IS NOT NULL,
    /// soft-deleted transactions are automatically excluded from inventory counts.
    /// </summary>
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _SqlExecutor;

        public SqlInventoryRepository(ISqlExecutor sqlExecutor)
        {
            _SqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<InventoryTransaction> AddTransactionAsync(InventoryTransaction transaction)
        {
            return await _SqlExecutor.ExecuteAsync<InventoryTransaction>(async (conn, trans) =>
            {
                const string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
                    VALUES
                        (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var transactionId = await conn.QuerySingleAsync<int>(sql, new
                {
                    transaction.ProductInstanceId,
                    transaction.Quantity,
                    transaction.StartedTimestamp,
                    transaction.CompletedTimestamp,
                    transaction.TypeCategory
                }, trans);

                transaction.TransactionId = transactionId;
                return transaction;
            });
        }

        /// <summary>
        /// EVAL: Soft-deletes a transaction to support the "undo" capability.
        /// Sets CompletedTimestamp to NULL instead of deleting the row.
        /// This preserves the full audit trail — the transaction record remains in the database
        /// but is excluded from inventory counts (which filter on CompletedTimestamp IS NOT NULL).
        /// This approach also allows "redo" in the future by setting CompletedTimestamp back.
        /// </summary>
        public async Task<bool> RemoveTransactionAsync(int transactionId)
        {
            return await _SqlExecutor.ExecuteAsync<bool>(async (conn, trans) =>
            {
                const string sql = @"
                    UPDATE [Transactions].[InventoryTransactions]
                    SET [CompletedTimestamp] = NULL
                    WHERE [TransactionId] = @TransactionId
                      AND [CompletedTimestamp] IS NOT NULL;";

                var rowsAffected = await conn.ExecuteAsync(sql, new { TransactionId = transactionId }, trans);
                return rowsAffected > 0;
            });
        }

        /// <summary>
        /// EVAL: Returns the net inventory count for a product by summing all completed transaction quantities.
        /// Positive quantities = added to inventory, negative quantities = removed from inventory.
        /// </summary>
        public async Task<decimal> GetInventoryCountAsync(int productInstanceId)
        {
            return await _SqlExecutor.ExecuteAsync<decimal>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT ISNULL(SUM([Quantity]), 0)
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [ProductInstanceId] = @ProductInstanceId
                      AND [CompletedTimestamp] IS NOT NULL;";

                return await conn.QuerySingleAsync<decimal>(sql, new { ProductInstanceId = productInstanceId }, trans);
            });
        }

        /// <summary>
        /// EVAL: Returns inventory counts grouped by product for all products matching a given attribute.
        /// Enables queries like "how many units of red products are in stock?"
        /// </summary>
        public async Task<IEnumerable<InventoryCountSummary>> GetInventoryCountsByAttributeAsync(string key, string value)
        {
            return await _SqlExecutor.ExecuteAsync<IEnumerable<InventoryCountSummary>>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT it.[ProductInstanceId], ISNULL(SUM(it.[Quantity]), 0) AS [Count]
                    FROM [Transactions].[InventoryTransactions] it
                    INNER JOIN [Instances].[ProductAttributes] pa
                        ON it.[ProductInstanceId] = pa.[InstanceId]
                    WHERE pa.[Key] = @Key
                      AND pa.[Value] = @Value
                      AND it.[CompletedTimestamp] IS NOT NULL
                    GROUP BY it.[ProductInstanceId];";

                return await conn.QueryAsync<InventoryCountSummary>(sql, new { Key = key, Value = value }, trans);
            });
        }
    }
}
