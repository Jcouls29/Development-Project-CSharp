using Dapper;
using Sparcpoint;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlInventoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(AddInventoryRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(request.Quantity));

            return await _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                const string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory)
                    OUTPUT INSERTED.TransactionId
                    VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), SYSUTCDATETIME(), @TypeCategory)";

                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    request.ProductInstanceId,
                    request.Quantity,
                    request.TypeCategory
                }, trans);
            });
        }

        public async Task<int> RemoveAsync(RemoveInventoryRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(request.Quantity));

            // EVAL: Removal is stored as a negative quantity row rather than
            // decrementing a counter. This preserves the full transaction history
            // and makes the "undo" operation (RemoveTransactionAsync) trivial.
            return await _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                const string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory)
                    OUTPUT INSERTED.TransactionId
                    VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), SYSUTCDATETIME(), @TypeCategory)";

                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    request.ProductInstanceId,
                    Quantity = -request.Quantity,   // negate to represent removal
                    request.TypeCategory
                }, trans);
            });
        }

        public async Task RemoveTransactionAsync(int transactionId)
        {
            // EVAL: This is the "undo" mechanic from the spec. Deleting a transaction
            // row is safe because inventory count is always derived from SUM(Quantity).
            // No cascading side effects — the product record is untouched.
            await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = @"
                    DELETE FROM [Transactions].[InventoryTransactions]
                    WHERE TransactionId = @TransactionId";

                await conn.ExecuteAsync(sql, new { TransactionId = transactionId }, trans);
            });
        }

        public async Task<decimal> GetInventoryCountAsync(int productInstanceId)
        {
            // EVAL: SUM over Quantity naturally handles both additions (positive)
            // and removals (negative) in a single query. COALESCE handles the
            // zero-transaction case where SUM would return NULL.
            return await _Executor.ExecuteAsync<decimal>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT COALESCE(SUM(Quantity), 0)
                    FROM [Transactions].[InventoryTransactions]
                    WHERE ProductInstanceId = @ProductInstanceId";

                return await conn.ExecuteScalarAsync<decimal>(sql, new { ProductInstanceId = productInstanceId }, trans);
            });
        }
    }
}
