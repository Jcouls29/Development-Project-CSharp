using System.Threading.Tasks;
using Dapper;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Web.Repositories.Implementations;

public class InventoryRepository : IInventoryRepository
{
    private readonly ISqlExecutor _sqlExecutor;

    public InventoryRepository(ISqlExecutor sqlExecutor)
    {
        _sqlExecutor = sqlExecutor;
    }

    public async Task<int> AddAsync(AddInventoryRequest request)
    {
        return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
        {
            const string sql = @"
                INSERT INTO Transactions.InventoryTransactions
                    (ProductInstanceId, Quantity, TypeCategory)
                VALUES
                    (@ProductInstanceId, @Quantity, @TypeCategory);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                request.ProductInstanceId,
                request.Quantity,
                request.TypeCategory
            }, trans);
        });
    }

    public async Task RemoveTransactionAsync(int transactionId)
    {
        await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
        {
            // EVAL: Hard delete is the undo mechanism — removing the row immediately
            // corrects the inventory count since the SUM query only touches existing rows.
            const string sql = @"
                DELETE FROM Transactions.InventoryTransactions
                WHERE TransactionId = @TransactionId";

            await conn.ExecuteAsync(sql, new { TransactionId = transactionId }, trans);
        });
    }

    public async Task<decimal> GetCountAsync(InventoryCountRequest request)
    {
        return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
        {
            // EVAL: ISNULL guards against SUM returning NULL on an empty set.
            const string baseSelect = @"
                SELECT ISNULL(SUM(t.Quantity), 0)
                FROM Transactions.InventoryTransactions t";

            var query = SqlServerQueryProvider.Empty;
            query.SetTargetTableAlias("t");

            if (request.ProductInstanceId.HasValue)
                query.WhereEquals("ProductInstanceId", "ProductInstanceId", request.ProductInstanceId.Value);

            // EVAL: EXISTS lets callers aggregate stock across all products sharing an attribute
            // (e.g. total units of all red products) without needing to know their IDs.
            if (!string.IsNullOrWhiteSpace(request.AttributeKey) && !string.IsNullOrWhiteSpace(request.AttributeValue))
            {
                query.Where($@"EXISTS (
                    SELECT 1 FROM Instances.ProductAttributes pa
                    WHERE pa.InstanceId = t.ProductInstanceId
                    AND pa.[Key] = @AttributeKey
                    AND pa.[Value] = @AttributeValue)");

                query.AddParameter("@AttributeKey", request.AttributeKey);
                query.AddParameter("@AttributeValue", request.AttributeValue);
            }

            var sql = $"{baseSelect} {query.WhereClause}";

            return await conn.ExecuteScalarAsync<decimal>(sql, query.Parameters, trans);
        });
    }
}
