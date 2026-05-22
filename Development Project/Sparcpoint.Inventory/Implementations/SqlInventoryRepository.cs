using Dapper;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Extensions;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    // EVAL: Inventory is modeled as an append-only ledger of signed-quantity transactions. The
    // "current count" of a product is SUM(Quantity), so "add 5" is a +5 row and "remove 3" is a -3
    // row. This makes requirement #6 (undo) trivially achievable: deleting the transaction row
    // removes its effect from the sum. No mutable stock-on-hand column needs to stay in sync.
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlInventoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        // EVAL: Accepts an IEnumerable of adjustments so the API can add/remove one product or many
        // in a single call (requirement #4). The entire batch runs inside one ISqlExecutor transaction
        // — partial failures roll back, so the inventory ledger is never left half-applied.
        public Task<IEnumerable<InventoryTransaction>> CreateTransactionsAsync(IEnumerable<InventoryAdjustment> adjustments)
        {
            PreConditions.ParameterNotNull(adjustments, nameof(adjustments));

            return _Executor.ExecuteAsync<IEnumerable<InventoryTransaction>>(async (conn, trans) =>
            {
                var results = new List<InventoryTransaction>();

                foreach (var adjustment in adjustments)
                {
                    var transaction = await conn.QuerySingleAsync<InventoryTransactionRow>(@"
                        INSERT INTO [Transactions].[InventoryTransactions]
                            ([ProductInstanceId], [Quantity], [TypeCategory])
                        VALUES (@ProductId, @Quantity, @TypeCategory);
                        SELECT [TransactionId], [ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory]
                        FROM [Transactions].[InventoryTransactions]
                        WHERE [TransactionId] = SCOPE_IDENTITY();",
                        new
                        {
                            ProductId = adjustment.ProductId,
                            Quantity = adjustment.Quantity,
                            TypeCategory = adjustment.TypeCategory?.Truncate(32)
                        },
                        trans);

                    results.Add(MapTransaction(transaction));
                }

                return results;
            });
        }

        // EVAL: Satisfies requirement #6. Because counts are SUM-based, removing the row removes
        // its effect on every count query that follows. Throws KeyNotFoundException when no row was
        // affected so the controller can translate it to HTTP 404.
        public Task DeleteTransactionAsync(int transactionId)
        {
            return _Executor.ExecuteAsync(async (conn, trans) =>
            {
                var affected = await conn.ExecuteAsync(@"
                    DELETE FROM [Transactions].[InventoryTransactions]
                    WHERE [TransactionId] = @TransactionId",
                    new { TransactionId = transactionId }, trans);

                if (affected == 0)
                    throw new KeyNotFoundException($"Transaction {transactionId} not found.");
            });
        }

        // EVAL: Satisfies requirement #5 — count by product id, by subset of metadata, or both.
        // Two code paths: a hand-written fast path for the common id-only lookup (single seek on
        // the IX_InventoryTransactions_ProductInstanceId_Quantity index), and a dynamic path using
        // SqlServerQueryProvider when attribute filters are involved.
        public Task<decimal> GetCountAsync(InventoryCountQuery query)
        {
            PreConditions.ParameterNotNull(query, nameof(query));

            return _Executor.ExecuteAsync<decimal>(async (conn, trans) =>
            {
                if (query.ProductId.HasValue && (query.AttributeFilters == null || query.AttributeFilters.Count == 0))
                {
                    return await conn.QuerySingleAsync<decimal?>(@"
                        SELECT ISNULL(SUM([Quantity]), 0)
                        FROM [Transactions].[InventoryTransactions]
                        WHERE [ProductInstanceId] = @ProductId",
                        new { ProductId = query.ProductId.Value }, trans) ?? 0m;
                }

                var provider = new SqlServerQueryProvider();

                if (query.ProductId.HasValue)
                {
                    var idParam = provider.GetNextParameterName("@ProductId");
                    provider.Where($"it.[ProductInstanceId] = {idParam}")
                            .AddParameter(idParam, query.ProductId.Value);
                }

                if (query.AttributeFilters != null)
                {
                    foreach (var kvp in query.AttributeFilters)
                    {
                        var keyParam = provider.GetNextParameterName("@AttrKey");
                        var valParam = provider.GetNextParameterName("@AttrVal");
                        provider.Where($"EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] pa WHERE pa.[InstanceId] = it.[ProductInstanceId] AND pa.[Key] = {keyParam} AND pa.[Value] = {valParam})")
                                .AddParameter(keyParam, kvp.Key)
                                .AddParameter(valParam, kvp.Value);
                    }
                }

                var sql = $@"
                    SELECT ISNULL(SUM(it.[Quantity]), 0)
                    FROM [Transactions].[InventoryTransactions] it
                    {provider.WhereClause}";

                return await conn.QuerySingleAsync<decimal?>(sql, provider.Parameters, trans) ?? 0m;
            });
        }

        private static InventoryTransaction MapTransaction(InventoryTransactionRow row) => new InventoryTransaction
        {
            TransactionId = row.TransactionId,
            ProductId = row.ProductInstanceId,
            Quantity = row.Quantity,
            StartedTimestamp = row.StartedTimestamp,
            CompletedTimestamp = row.CompletedTimestamp,
            TypeCategory = row.TypeCategory
        };

        private class InventoryTransactionRow
        {
            public int TransactionId { get; set; }
            public int ProductInstanceId { get; set; }
            public decimal Quantity { get; set; }
            public DateTime StartedTimestamp { get; set; }
            public DateTime? CompletedTimestamp { get; set; }
            public string TypeCategory { get; set; }
        }
    }
}
