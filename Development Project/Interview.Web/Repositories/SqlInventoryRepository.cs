using Interview.Web.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Interview.Web.Repositories
{
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public SqlInventoryRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<InventoryTransaction> AddInventoryAsync(Guid productId, int delta, string note)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                var txId = Guid.NewGuid();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "INSERT INTO dbo.InventoryTransactions (Id, ProductId, Timestamp, Delta, Note) VALUES (@Id, @ProductId, @Timestamp, @Delta, @Note)";
                    var idp = cmd.CreateParameter(); idp.ParameterName = "@Id"; idp.Value = txId; cmd.Parameters.Add(idp);
                    var pid = cmd.CreateParameter(); pid.ParameterName = "@ProductId"; pid.Value = productId; cmd.Parameters.Add(pid);
                    var ts = cmd.CreateParameter(); ts.ParameterName = "@Timestamp"; ts.Value = DateTime.UtcNow; cmd.Parameters.Add(ts);
                    var d = cmd.CreateParameter(); d.ParameterName = "@Delta"; d.Value = delta; cmd.Parameters.Add(d);
                    var n = cmd.CreateParameter(); n.ParameterName = "@Note"; n.Value = (object)note ?? DBNull.Value; cmd.Parameters.Add(n);
                    ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "UPDATE dbo.Products SET Quantity = ISNULL(Quantity,0) + @Delta WHERE Id = @ProductId";
                    var dd = cmd.CreateParameter(); dd.ParameterName = "@Delta"; dd.Value = delta; cmd.Parameters.Add(dd);
                    var pid2 = cmd.CreateParameter(); pid2.ParameterName = "@ProductId"; pid2.Value = productId; cmd.Parameters.Add(pid2);
                    var rows = ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                    if (rows == 0) throw new InvalidOperationException("Product not found");
                }

                return new InventoryTransaction { Id = txId, ProductId = productId, Delta = delta, Note = note };
            });
        }

        public Task<int?> GetQuantityAsync(Guid productId)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT Quantity FROM dbo.Products WHERE Id = @Id";
                    var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = productId; cmd.Parameters.Add(pId);
                    var val = ((System.Data.Common.DbCommand)cmd).ExecuteScalar();
                    if (val == null || val == DBNull.Value) return (int?)null;
                    return Convert.ToInt32(val);
                }
            });
        }

        public Task<bool> UndoLastInventoryAsync(Guid productId)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                Guid lastId = Guid.Empty;
                int lastDelta = 0;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT TOP 1 Id, Delta FROM dbo.InventoryTransactions WHERE ProductId = @ProductId ORDER BY Timestamp DESC, Id DESC";
                    var pid = cmd.CreateParameter(); pid.ParameterName = "@ProductId"; pid.Value = productId; cmd.Parameters.Add(pid);
                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        if (!reader.Read()) return false;
                        lastId = reader.GetGuid(0);
                        lastDelta = reader.GetInt32(1);
                    }
                }

                var undoId = Guid.NewGuid();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "INSERT INTO dbo.InventoryTransactions (Id, ProductId, Timestamp, Delta, Note) VALUES (@Id, @ProductId, @Timestamp, @Delta, @Note)";
                    var idp = cmd.CreateParameter(); idp.ParameterName = "@Id"; idp.Value = undoId; cmd.Parameters.Add(idp);
                    var pid = cmd.CreateParameter(); pid.ParameterName = "@ProductId"; pid.Value = productId; cmd.Parameters.Add(pid);
                    var ts = cmd.CreateParameter(); ts.ParameterName = "@Timestamp"; ts.Value = DateTime.UtcNow; cmd.Parameters.Add(ts);
                    var d = cmd.CreateParameter(); d.ParameterName = "@Delta"; d.Value = -lastDelta; cmd.Parameters.Add(d);
                    var n = cmd.CreateParameter(); n.ParameterName = "@Note"; n.Value = (object)$"UNDO of {lastId}" ?? DBNull.Value; cmd.Parameters.Add(n);
                    ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "UPDATE dbo.Products SET Quantity = ISNULL(Quantity,0) + @Delta WHERE Id = @ProductId";
                    var dd = cmd.CreateParameter(); dd.ParameterName = "@Delta"; dd.Value = -lastDelta; cmd.Parameters.Add(dd);
                    var pid2 = cmd.CreateParameter(); pid2.ParameterName = "@ProductId"; pid2.Value = productId; cmd.Parameters.Add(pid2);
                    var rows = ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                    if (rows == 0) throw new InvalidOperationException("Product not found");
                }

                return true;
            });
        }

        public Task<IEnumerable<InventoryTransaction>> AddInventoryBatchAsync(IEnumerable<InventoryTransaction> transactions)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                var added = new List<InventoryTransaction>();

                // Ensure transactions table exists
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = @"IF OBJECT_ID('dbo.InventoryTransactions','U') IS NULL
                        BEGIN
                            CREATE TABLE dbo.InventoryTransactions (
                                Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                                ProductId UNIQUEIDENTIFIER NOT NULL,
                                Timestamp DATETIME2 NOT NULL,
                                Delta INT NOT NULL,
                                Note NVARCHAR(1000) NULL
                            )
                        END";
                    ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                }

                foreach (var t in transactions)
                {
                    var txId = t.Id == Guid.Empty ? Guid.NewGuid() : t.Id;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        cmd.CommandText = "INSERT INTO dbo.InventoryTransactions (Id, ProductId, Timestamp, Delta, Note) VALUES (@Id, @ProductId, @Timestamp, @Delta, @Note)";
                        var idp = cmd.CreateParameter(); idp.ParameterName = "@Id"; idp.Value = txId; cmd.Parameters.Add(idp);
                        var pid = cmd.CreateParameter(); pid.ParameterName = "@ProductId"; pid.Value = t.ProductId; cmd.Parameters.Add(pid);
                        var ts = cmd.CreateParameter(); ts.ParameterName = "@Timestamp"; ts.Value = t.Timestamp == default ? DateTime.UtcNow : t.Timestamp; cmd.Parameters.Add(ts);
                        var d = cmd.CreateParameter(); d.ParameterName = "@Delta"; d.Value = t.Delta; cmd.Parameters.Add(d);
                        var n = cmd.CreateParameter(); n.ParameterName = "@Note"; n.Value = (object)t.Note ?? DBNull.Value; cmd.Parameters.Add(n);
                        ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        cmd.CommandText = "UPDATE dbo.Products SET Quantity = ISNULL(Quantity,0) + @Delta WHERE Id = @ProductId";
                        var dd = cmd.CreateParameter(); dd.ParameterName = "@Delta"; dd.Value = t.Delta; cmd.Parameters.Add(dd);
                        var pid2 = cmd.CreateParameter(); pid2.ParameterName = "@ProductId"; pid2.Value = t.ProductId; cmd.Parameters.Add(pid2);
                        var rows = ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                        if (rows == 0) throw new InvalidOperationException($"Product not found: {t.ProductId}");
                    }

                    added.Add(new InventoryTransaction { Id = txId, ProductId = t.ProductId, Delta = t.Delta, Note = t.Note, Timestamp = t.Timestamp == default ? DateTime.UtcNow : t.Timestamp });
                }

                return (IEnumerable<InventoryTransaction>)added;
            });
        }

        public Task<bool> RemoveTransactionAsync(Guid transactionId)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                Guid prodId = Guid.Empty;
                int delta = 0;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT ProductId, Delta FROM dbo.InventoryTransactions WHERE Id = @Id";
                    var idp = cmd.CreateParameter(); idp.ParameterName = "@Id"; idp.Value = transactionId; cmd.Parameters.Add(idp);
                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        if (!reader.Read()) return false;
                        prodId = reader.GetGuid(0);
                        delta = reader.GetInt32(1);
                    }
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "DELETE FROM dbo.InventoryTransactions WHERE Id = @Id";
                    var idp = cmd.CreateParameter(); idp.ParameterName = "@Id"; idp.Value = transactionId; cmd.Parameters.Add(idp);
                    ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "UPDATE dbo.Products SET Quantity = ISNULL(Quantity,0) - @Delta WHERE Id = @ProductId";
                    var dd = cmd.CreateParameter(); dd.ParameterName = "@Delta"; dd.Value = delta; cmd.Parameters.Add(dd);
                    var pid2 = cmd.CreateParameter(); pid2.ParameterName = "@ProductId"; pid2.Value = prodId; cmd.Parameters.Add(pid2);
                    var rows = ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                    if (rows == 0) throw new InvalidOperationException("Product not found");
                }

                return true;
            });
        }
    }
}
