using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly ISqlExecutor _SqlExecutor;

        public InventoryTransactionService(ISqlExecutor sqlExecutor)
        {
            _SqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<int> RecordInventoryTransactionAsync(InventoryRequest request)
        {
            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                const string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity, TypeCategory)
                    VALUES (@ProductId, @Quantity, @Type);
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@ProductId", request.ProductInstanceId);
                    cmd.Parameters.AddWithValue("@Quantity", request.Quantity);
                    cmd.Parameters.AddWithValue("@Type", request.TypeCategory ?? (object)DBNull.Value);
                    return (int)await cmd.ExecuteScalarAsync();
                }
            });
        }

        public async Task RecordInventoryTransactionsAsync(IEnumerable<InventoryRequest> requests)
        {
            await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                const string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity, TypeCategory)
                    VALUES (@ProductId, @Quantity, @Type)";
                
                foreach (var request in requests)
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Transaction = (SqlTransaction)transaction;
                        cmd.Parameters.AddWithValue("@ProductId", request.ProductInstanceId);
                        cmd.Parameters.AddWithValue("@Quantity", request.Quantity);
                        cmd.Parameters.AddWithValue("@Type", request.TypeCategory ?? (object)DBNull.Value);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            });
        }

        public async Task<decimal> GetProductStockAsync(int productId)
        {
            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                const string sql = "SELECT ISNULL(SUM(Quantity), 0) FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ProductId AND IsDeleted = 0";
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    return (decimal)await cmd.ExecuteScalarAsync();
                }
            });
        }

        public async Task<decimal> GetStockByMetadataAsync(string key, string value)
        {
            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                const string sql = @"
                    SELECT ISNULL(SUM(it.Quantity), 0)
                    FROM [Transactions].[InventoryTransactions] it
                    JOIN [Instances].[ProductAttributes] pa ON it.ProductInstanceId = pa.InstanceId
                    WHERE pa.[Key] = @Key AND pa.[Value] = @Value AND it.IsDeleted = 0";
                
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@Key", key);
                    cmd.Parameters.AddWithValue("@Value", value);
                    return (decimal)await cmd.ExecuteScalarAsync();
                }
            });
        }

        public async Task UndoInventoryTransactionAsync(int transactionId)
        {
            await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                const string sql = "UPDATE [Transactions].[InventoryTransactions] SET IsDeleted = 1 WHERE TransactionId = @Id";
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@Id", transactionId);
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }
    }
}
