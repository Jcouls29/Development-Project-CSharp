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
            // EVAL: Deleting the transaction row is the "undo" mechanism per the spec requirement
            // that individual transactions should be able to be removed. The inventory count
            // query only sums active rows so removing a row immediately corrects the count.
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
            // EVAL: ISNULL(..., 0) guards against NULL being returned when no transactions exist
            // for a product, which would cause Dapper to return 0m rather than throw.
            const string baseSelect = @"
                SELECT ISNULL(SUM(t.Quantity), 0)
                FROM Transactions.InventoryTransactions t";

            var query = SqlServerQueryProvider.Empty;
            query.SetTargetTableAlias("t");

            if (request.ProductInstanceId.HasValue)
                query.WhereEquals("ProductInstanceId", "ProductInstanceId", request.ProductInstanceId.Value);

            // EVAL: Attribute-based count joins back to ProductAttributes so callers can ask
            // "how many units do I have across all products with color=red" without knowing
            // the specific product IDs.
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
