using Sparcpoint.Inventory.Application.Interfaces;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Application.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _sql;

        public InventoryRepository(ISqlExecutor sql)
        {
            _sql = sql;
        }

        public Task<decimal> GetInventoryCountAsync(int productId)
        {
            return _sql.ExecuteAsync(async (conn, tx) =>
            {
                var cmd = conn.CreateCommand();
                cmd.Transaction = tx;

                cmd.CommandText = @"
SELECT ISNULL(SUM(Quantity), 0)
FROM Transactions.InventoryTransactions
WHERE ProductInstanceId = @ProductId
AND CompletedTimestamp IS NOT NULL";

                var p = cmd.CreateParameter();
                p.ParameterName = "@ProductId";
                p.Value = productId;

                cmd.Parameters.Add(p);

                var result = cmd.ExecuteScalar();

                return Convert.ToDecimal(result);
            });
        }

        public Task AddInventoryAsync(List<int> productIds, decimal quantity)
        {
            return _sql.ExecuteAsync(async (conn, tx) =>
            {
                foreach (var productId in productIds)
                {
                    var cmd = conn.CreateCommand();
                    cmd.Transaction = tx;

                    cmd.CommandText = @"
INSERT INTO Transactions.InventoryTransactions
(ProductInstanceId, Quantity, TypeCategory, CompletedTimestamp)
VALUES (@ProductId, @Quantity, @Type, SYSUTCDATETIME())";

                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@ProductId";
                    p1.Value = productId;

                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@Quantity";
                    p2.Value = quantity;

                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@Type";
                    p3.Value = quantity >= 0 ? "ADD" : "REMOVE";

                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    cmd.Parameters.Add(p3);

                    cmd.ExecuteNonQuery();
                }
            });
        }

        public Task DeleteTransactionAsync(int transactionId)
        {
            return _sql.ExecuteAsync(async (conn, tx) =>
            {
                var cmd = conn.CreateCommand();
                cmd.Transaction = tx;

                cmd.CommandText = @"
DELETE FROM Transactions.InventoryTransactions
WHERE TransactionId = @Id";

                var p = cmd.CreateParameter();
                p.ParameterName = "@Id";
                p.Value = transactionId;

                cmd.Parameters.Add(p);

                cmd.ExecuteNonQuery();
            });
        }
    }
}
