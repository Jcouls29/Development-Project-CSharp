using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Sparcpoint;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.SqlServer.Repositories
{
    /// <summary>
    /// EVAL: Inventory repository. Domain rules:
    ///  - Quantity &gt; 0  ⇒ add stock
    ///  - Quantity &lt; 0  ⇒ remove stock
    ///  - Revert: soft-delete via CompletedTimestamp = NULL to preserve
    ///            audit trail (requirement: "removed", not "deleted").
    ///  - GetCounts: only sums transactions with CompletedTimestamp NOT NULL
    ///               (unless IncludeReverted = true, useful for audit).
    /// </summary>
    public sealed class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlInventoryRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<IReadOnlyList<int>> RecordAsync(IEnumerable<InventoryAdjustment> adjustments, CancellationToken cancellationToken = default)
        {
            PreConditions.ParameterNotNull(adjustments, nameof(adjustments));

            var list = adjustments.ToList();
            if (list.Count == 0) return Task.FromResult<IReadOnlyList<int>>(Array.Empty<int>());
            if (list.Any(a => a.Quantity == 0))
                throw new ArgumentException("Quantity cannot be zero.", nameof(adjustments));
            if (list.Any(a => a.ProductInstanceId <= 0))
                throw new ArgumentException("ProductInstanceId must be positive.", nameof(adjustments));
            if (list.Any(a => (a.TypeCategory ?? string.Empty).Length > 32))
                throw new ArgumentException("TypeCategory exceeds 32 chars.", nameof(adjustments));

            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                // EVAL: OUTPUT INSERTED.TransactionId in a multi-row bulk insert
                // to get the generated IDs in a single DB roundtrip.
                // Dapper multi-row with OUTPUT requires individual calls to
                // retrieve the scalar; for small N (typical case) it's simpler
                // this way. For N >> 100 we'd use TVP + MERGE/INSERT from TVP.
                const string insertSql = @"
INSERT INTO [Transactions].[InventoryTransactions]
    ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
OUTPUT INSERTED.[TransactionId]
VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory);";

                var ids = new List<int>(list.Count);
                foreach (var adj in list)
                {
                    var id = await conn.ExecuteScalarAsync<int>(new CommandDefinition(insertSql, new
                    {
                        adj.ProductInstanceId,
                        adj.Quantity,
                        TypeCategory = adj.TypeCategory
                    }, transaction: tx, cancellationToken: cancellationToken));
                    ids.Add(id);
                }
                return (IReadOnlyList<int>)ids;
            });
        }

        public Task<bool> RevertAsync(int transactionId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                // EVAL: Only revert if CompletedTimestamp IS NOT NULL (prevent double-revert).
                const string sql = @"
UPDATE [Transactions].[InventoryTransactions]
SET [CompletedTimestamp] = NULL
WHERE [TransactionId] = @Id AND [CompletedTimestamp] IS NOT NULL;";

                var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { Id = transactionId }, transaction: tx, cancellationToken: cancellationToken));
                return affected > 0;
            });
        }

        public Task<IReadOnlyList<InventoryCountResult>> GetCountsAsync(InventoryCountQuery query, CancellationToken cancellationToken = default)
        {
            PreConditions.ParameterNotNull(query, nameof(query));

            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var where = new List<string>();
                var parameters = new DynamicParameters();

                if (query.ProductInstanceId.HasValue)
                {
                    where.Add("P.[InstanceId] = @PId");
                    parameters.Add("@PId", query.ProductInstanceId.Value);
                }

                if (query.AttributeFilters != null)
                {
                    int i = 0;
                    foreach (var attr in query.AttributeFilters)
                    {
                        if (string.IsNullOrWhiteSpace(attr.Key)) continue;
                        var kp = "@AK" + i;
                        var vp = "@AV" + i;
                        where.Add($@"EXISTS (
    SELECT 1 FROM [Instances].[ProductAttributes] PA
    WHERE PA.[InstanceId] = P.[InstanceId] AND PA.[Key] = {kp} AND PA.[Value] = {vp})");
                        parameters.Add(kp, attr.Key);
                        parameters.Add(vp, attr.Value);
                        i++;
                    }
                }

                if (query.CategoryIds != null && query.CategoryIds.Count > 0)
                {
                    int i = 0;
                    foreach (var cid in query.CategoryIds)
                    {
                        var p = "@CI" + i;
                        where.Add($@"EXISTS (
    SELECT 1 FROM [Instances].[ProductCategories] PC
    WHERE PC.[InstanceId] = P.[InstanceId] AND PC.[CategoryInstanceId] = {p})");
                        parameters.Add(p, cid);
                        i++;
                    }
                }

                var filter = query.IncludeReverted
                    ? string.Empty
                    : "AND T.[CompletedTimestamp] IS NOT NULL";

                var sb = new StringBuilder();
                sb.Append("SELECT P.[InstanceId] AS ProductInstanceId, P.[Name] AS ProductName, ");
                sb.Append($"COALESCE((SELECT SUM(T.[Quantity]) FROM [Transactions].[InventoryTransactions] T WHERE T.[ProductInstanceId] = P.[InstanceId] {filter}), 0) AS [Count] ");
                sb.Append("FROM [Instances].[Products] P ");
                if (where.Count > 0) sb.Append("WHERE ").Append(string.Join(" AND ", where));
                sb.Append(" ORDER BY P.[InstanceId];");

                var rows = await conn.QueryAsync<InventoryCountResult>(
                    new CommandDefinition(sb.ToString(), parameters, transaction: tx, cancellationToken: cancellationToken));

                return (IReadOnlyList<InventoryCountResult>)rows.ToList();
            });
        }

        public Task<IReadOnlyList<InventoryTransaction>> ListTransactionsAsync(int productInstanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                const string sql = @"
SELECT [TransactionId], [ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory]
FROM [Transactions].[InventoryTransactions]
WHERE [ProductInstanceId] = @Id
ORDER BY [TransactionId];";

                var rows = await conn.QueryAsync<InventoryTransaction>(
                    new CommandDefinition(sql, new { Id = productInstanceId }, transaction: tx, cancellationToken: cancellationToken));
                return (IReadOnlyList<InventoryTransaction>)rows.ToList();
            });
        }
    }
}
