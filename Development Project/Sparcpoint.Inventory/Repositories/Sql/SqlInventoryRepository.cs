using Dapper;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories.Sql
{
    public sealed class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlInventoryRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<int> RecordTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            return _executor.ExecuteAsync(async (conn, tx) =>
                await conn.ExecuteScalarAsync<int>(Queries.InsertTransaction, transaction, tx));
        }

        public Task<IReadOnlyList<int>> RecordTransactionsAsync(IReadOnlyList<InventoryTransaction> transactions, CancellationToken cancellationToken = default)
        {
            if (transactions is null || transactions.Count == 0) throw new ArgumentException("At least one transaction is required.", nameof(transactions));

            // EVAL: Batched inside a single SqlExecutor transaction — all-or-nothing semantics.
            return _executor.ExecuteAsync<IReadOnlyList<int>>(async (conn, tx) =>
            {
                var ids = new List<int>(transactions.Count);
                foreach (var t in transactions)
                {
                    var id = await conn.ExecuteScalarAsync<int>(Queries.InsertTransaction, t, tx);
                    ids.Add(id);
                }
                return ids;
            });
        }

        public Task<bool> RemoveTransactionAsync(int transactionId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                var affected = await conn.ExecuteAsync(Queries.DeleteTransaction, new { TransactionId = transactionId }, tx);
                return affected > 0;
            });
        }

        public Task<InventoryCount> GetCountAsync(int productInstanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
                await conn.QuerySingleAsync<InventoryCount>(Queries.CountByProduct, new { ProductInstanceId = productInstanceId }, tx));
        }

        public Task<IReadOnlyList<InventoryCount>> GetCountsByAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
        {
            if (attribute is null) throw new ArgumentNullException(nameof(attribute));
            return _executor.ExecuteAsync<IReadOnlyList<InventoryCount>>(async (conn, tx) =>
                (await conn.QueryAsync<InventoryCount>(Queries.CountByAttribute, new { attribute.Key, attribute.Value }, tx)).ToArray());
        }
    }
}
