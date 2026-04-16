using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Interview.Web.Models;
using Sparcpoint.SqlServer.Abstractions;
using Interview.Web.Repositories.Interfaces;

namespace Interview.Web.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public InventoryRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<int> AddTransactionAsync(InventoryTransactionRequest request)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory)
                    VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), SYSUTCDATETIME(), @TypeCategory);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                return await connection.ExecuteScalarAsync<int>(sql, new
                {
                    request.ProductInstanceId,
                    request.Quantity,
                    request.TypeCategory
                }, transaction);
            });
        }

        public async Task<decimal> GetStockAsync(int productId)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var sql = "SELECT ISNULL(SUM(Quantity), 0) FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ProductId";
                return await connection.ExecuteScalarAsync<decimal>(sql, new { ProductId = productId }, transaction);
            });
        }

        public async Task<decimal> GetStockByMetadataAsync(string key, string value)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var sql = @"
                    SELECT ISNULL(SUM(it.Quantity), 0)
                    FROM [Transactions].[InventoryTransactions] it
                    INNER JOIN [Instances].[ProductAttributes] pa ON it.ProductInstanceId = pa.InstanceId
                    WHERE pa.[Key] = @Key AND pa.Value = @Value";

                return await connection.ExecuteScalarAsync<decimal>(sql, new { Key = key, Value = value }, transaction);
            });
        }

        public async Task<bool> RemoveTransactionAsync(int transactionId)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var sql = "DELETE FROM [Transactions].[InventoryTransactions] WHERE TransactionId = @TransactionId";
                var affected = await connection.ExecuteAsync(sql, new { TransactionId = transactionId }, transaction);
                return affected > 0;
            });
        }
    }
}
