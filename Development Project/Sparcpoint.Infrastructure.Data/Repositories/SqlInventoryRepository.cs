using Sparcpoint.SqlServer.Abstractions;
using Microsoft.Extensions.Logging;
using Sparcpoint.Application.Repositories;
using System;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Data.Repositories
{
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;
        private readonly ILogger<SqlInventoryRepository> _logger;

        public SqlInventoryRepository(ISqlExecutor executor, ILogger<SqlInventoryRepository> logger = null)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _logger = logger;
        }

        public async Task<int> RecordTransactionAsync(int productInstanceId, decimal quantity, string transactionType, int? relatedTransactionId = null)
        {
            int newTxId = 0;

            try
            {
                await _executor.ExecuteAsync(async (conn, tx) =>
                {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "INSERT INTO [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity, TypeCategory) VALUES (@ProductId, @Quantity, @Type); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    var pProd = cmd.CreateParameter(); pProd.ParameterName = "@ProductId"; pProd.Value = productInstanceId; cmd.Parameters.Add(pProd);
                    var pQty = cmd.CreateParameter(); pQty.ParameterName = "@Quantity"; pQty.Value = quantity; cmd.Parameters.Add(pQty);
                    var pType = cmd.CreateParameter(); pType.ParameterName = "@Type"; pType.Value = transactionType ?? string.Empty; cmd.Parameters.Add(pType);

                    var res = await ((System.Data.Common.DbCommand)cmd).ExecuteScalarAsync();
                    newTxId = (res == null || res == DBNull.Value) ? 0 : Convert.ToInt32(res);
                }
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to record transaction for productInstanceId {ProductInstanceId}", productInstanceId);
                throw;
            }

            return newTxId;
        }

        public async Task<bool> MarkAsReversedAsync(int transactionId)
        {
            // The existing schema does not have a Reversed column. As a best-effort we set CompletedTimestamp to now.
            try
            {
                await _executor.ExecuteAsync(async (conn, tx) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "UPDATE [Transactions].[InventoryTransactions] SET CompletedTimestamp = SYSUTCDATETIME() WHERE TransactionId = @Id";
                        var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = transactionId; cmd.Parameters.Add(pId);
                        await ((System.Data.Common.DbCommand)cmd).ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to mark transaction {TransactionId} as reversed/completed", transactionId);
                throw;
            }

            return true;
        }

        public async Task<int> UndoTransactionAsync(int transactionId)
        {
            if (transactionId <= 0) throw new ArgumentException("transactionId");

            int newTxId = 0;

            await _executor.ExecuteAsync(async (conn, tx) =>
            {
                // Read original transaction
                int productId = 0;
                decimal quantity = 0;
                string originalType = null;

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "SELECT ProductInstanceId, Quantity, TypeCategory FROM [Transactions].[InventoryTransactions] WHERE TransactionId = @Id";
                    var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = transactionId; cmd.Parameters.Add(pId);

                    using (var reader = await ((System.Data.Common.DbCommand)cmd).ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            throw new InvalidOperationException("Transaction not found");

                        productId = reader.GetInt32(0);
                        quantity = reader.GetDecimal(1);
                        originalType = reader.IsDBNull(2) ? null : reader.GetString(2);
                    }
                }

                // Insert compensating transaction (negative quantity)
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "INSERT INTO [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity, TypeCategory) VALUES (@ProductId, @Quantity, @Type); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    var pNewProd = cmd.CreateParameter(); pNewProd.ParameterName = "@ProductId"; pNewProd.Value = productId; cmd.Parameters.Add(pNewProd);
                    var pNewQty = cmd.CreateParameter(); pNewQty.ParameterName = "@Quantity"; pNewQty.Value = -quantity; cmd.Parameters.Add(pNewQty);
                    var pNewType = cmd.CreateParameter(); pNewType.ParameterName = "@Type"; pNewType.Value = (originalType == null ? "Reversal" : "Reversal:" + originalType); cmd.Parameters.Add(pNewType);

                    var res = await ((System.Data.Common.DbCommand)cmd).ExecuteScalarAsync();
                    newTxId = (res == null || res == DBNull.Value) ? 0 : Convert.ToInt32(res);
                }

                // Optionally mark original transaction completed
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "UPDATE [Transactions].[InventoryTransactions] SET CompletedTimestamp = SYSUTCDATETIME() WHERE TransactionId = @Id";
                    var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = transactionId; cmd.Parameters.Add(pId);
                    await ((System.Data.Common.DbCommand)cmd).ExecuteNonQueryAsync();
                }
            });

            return newTxId;
        }

        public async Task<Guid> UndoTransactionAsync(Guid transactionId)
        {
            if (transactionId == Guid.Empty) throw new ArgumentException("transactionId");

            Guid newTxId = Guid.Empty;

            await _executor.ExecuteAsync(async (conn, tx) =>
            {
                // Read the original transaction
                Guid productId;
                int quantity;
                bool reversed;
                string originalType = null;

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "SELECT ProductId, Quantity, Reversed, TransactionType FROM InventoryTransaction WHERE Id = @Id";
                    var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = transactionId; cmd.Parameters.Add(pId);

                    using (var reader = await ((System.Data.Common.DbCommand)cmd).ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            throw new InvalidOperationException("Transaction not found");

                        productId = reader.GetGuid(0);
                        quantity = reader.GetInt32(1);
                        reversed = reader.GetBoolean(2);
                        originalType = reader.IsDBNull(3) ? null : reader.GetString(3);
                    }
                }

                if (reversed)
                    throw new InvalidOperationException("Transaction already reversed");

                // Insert compensating transaction (opposite quantity)
                newTxId = Guid.NewGuid();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "INSERT INTO InventoryTransaction (Id, ProductId, Quantity, TransactionType, Timestamp, Reversed, RelatedTransactionId) VALUES (@Id, @ProductId, @Quantity, @Type, SYSUTCDATETIME(), 0, @Related)";

                    var pNewId = cmd.CreateParameter(); pNewId.ParameterName = "@Id"; pNewId.Value = newTxId; cmd.Parameters.Add(pNewId);
                    var pProd = cmd.CreateParameter(); pProd.ParameterName = "@ProductId"; pProd.Value = productId; cmd.Parameters.Add(pProd);
                    var pQty = cmd.CreateParameter(); pQty.ParameterName = "@Quantity"; pQty.Value = -quantity; cmd.Parameters.Add(pQty);
                    var pType = cmd.CreateParameter(); pType.ParameterName = "@Type"; pType.Value = (originalType == null ? "Reversal" : "Reversal:" + originalType); cmd.Parameters.Add(pType);
                    var pRel = cmd.CreateParameter(); pRel.ParameterName = "@Related"; pRel.Value = transactionId; cmd.Parameters.Add(pRel);

                    await ((System.Data.Common.DbCommand)cmd).ExecuteNonQueryAsync();
                }

                // Mark original as reversed
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "UPDATE InventoryTransaction SET Reversed = 1 WHERE Id = @Id";
                    var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = transactionId; cmd.Parameters.Add(pId);
                    await ((System.Data.Common.DbCommand)cmd).ExecuteNonQueryAsync();
                }
            });

            return newTxId;
        }
    }
}
