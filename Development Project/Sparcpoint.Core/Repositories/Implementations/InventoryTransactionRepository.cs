using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Core.Repositories.Implementations
{
    // EVAL: Repository Implementation - Uses raw ADO.NET for database operations
    // EVAL: Dependency Injection - Constructor injection for testability
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public InventoryTransactionRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<InventoryTransaction> GetByIdAsync(int transactionId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT TransactionId, ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory
                    FROM Transactions.InventoryTransactions
                    WHERE TransactionId = @TransactionId";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@TransactionId", transactionId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var transaction = InventoryTransaction.Load(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetDecimal(2),
                                reader.GetDateTime(3),
                                reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                reader.IsDBNull(5) ? null : reader.GetString(5));
                            return transaction;
                        }
                    }
                }

                return null;
            });
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productInstanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT TransactionId, ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory
                    FROM Transactions.InventoryTransactions
                    WHERE ProductInstanceId = @ProductInstanceId
                    ORDER BY StartedTimestamp DESC";

                var transactions = new List<InventoryTransaction>();

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@ProductInstanceId", productInstanceId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var transaction = InventoryTransaction.Load(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetDecimal(2),
                                reader.GetDateTime(3),
                                reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                reader.IsDBNull(5) ? null : reader.GetString(5));
                            transactions.Add(transaction);
                        }
                    }
                }

                return transactions;
            });
        }

        public async Task<int> AddAsync(InventoryTransaction transaction)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    INSERT INTO Transactions.InventoryTransactions (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory)
                    OUTPUT INSERTED.TransactionId
                    VALUES (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory)";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@ProductInstanceId", transaction.ProductInstanceId);
                    cmd.Parameters.AddWithValue("@Quantity", transaction.Quantity);
                    cmd.Parameters.AddWithValue("@StartedTimestamp", transaction.StartedTimestamp);
                    cmd.Parameters.AddWithValue("@CompletedTimestamp", transaction.CompletedTimestamp ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TypeCategory", transaction.TypeCategory ?? (object)DBNull.Value);

                    var transactionId = (int)await cmd.ExecuteScalarAsync();
                    transaction.SetTransactionId(transactionId);
                    return transactionId;
                }
            });
        }

        public async Task CompleteTransactionAsync(int transactionId)
        {
            await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    UPDATE Transactions.InventoryTransactions
                    SET CompletedTimestamp = @CompletedTimestamp
                    WHERE TransactionId = @TransactionId AND CompletedTimestamp IS NULL";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                    cmd.Parameters.AddWithValue("@CompletedTimestamp", DateTime.UtcNow);
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task<IEnumerable<InventoryTransaction>> GetUncompletedTransactionsAsync()
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT TransactionId, ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory
                    FROM Transactions.InventoryTransactions
                    WHERE CompletedTimestamp IS NULL
                    ORDER BY StartedTimestamp";

                var transactions = new List<InventoryTransaction>();

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var transaction = InventoryTransaction.Load(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetDecimal(2),
                                reader.GetDateTime(3),
                                reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                reader.IsDBNull(5) ? null : reader.GetString(5));
                            transactions.Add(transaction);
                        }
                    }
                }

                return transactions;
            });
        }
    }
}
