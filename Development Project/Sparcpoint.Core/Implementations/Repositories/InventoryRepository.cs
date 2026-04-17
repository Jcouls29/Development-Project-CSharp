using Sparcpoint.Interfaces;
using Sparcpoint.Models;
using Sparcpoint.Models.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Sparcpoint.Implementations.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;

        public InventoryRepository(ISqlExecutor executor)
        {
            _executor = executor;
        }

        // EVAL: Soporta múltiples productos en una sola transacción
        public async Task AddAsync(InventoryUpdateRequest request)
        {
            await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        (ProductInstanceId, Quantity, TypeCategory, CompletedTimestamp)
                    VALUES 
                        (@ProductInstanceId, @Quantity, 'ADD', SYSUTCDATETIME());";

                foreach (var productId in request.ProductInstanceIds)
                {
                    await conn.ExecuteAsync(sql, new
                    {
                        ProductInstanceId = productId,
                        request.Quantity
                    }, trans);
                }
            });
        }

        public async Task RemoveAsync(InventoryUpdateRequest request)
        {
            await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        (ProductInstanceId, Quantity, TypeCategory, CompletedTimestamp)
                    VALUES 
                        (@ProductInstanceId, @Quantity, 'REMOVE', SYSUTCDATETIME());";

                foreach (var productId in request.ProductInstanceIds)
                {
                    await conn.ExecuteAsync(sql, new
                    {
                        ProductInstanceId = productId,
                        request.Quantity
                    }, trans);
                }
            });
        }

        // EVAL: Calcula conteo neto sumando ADD y restando REMOVE
        public async Task<decimal> GetCountAsync(int productInstanceId)
        {
            return await _executor.ExecuteAsync<decimal>(async (conn, trans) =>
            {
                var sql = @"
                    SELECT ISNULL(SUM(
                        CASE 
                            WHEN TypeCategory = 'ADD' THEN Quantity 
                            WHEN TypeCategory = 'REMOVE' THEN -Quantity 
                            ELSE 0 
                        END), 0)
                    FROM [Transactions].[InventoryTransactions]
                    WHERE ProductInstanceId = @ProductInstanceId";

                return await conn.QuerySingleAsync<decimal>(
                    sql, new { ProductInstanceId = productInstanceId }, trans);
            });
        }

        // EVAL: Conteo filtrado por metadata del producto
        public async Task<decimal> GetCountByAttributeAsync(string key, string value)
        {
            return await _executor.ExecuteAsync<decimal>(async (conn, trans) =>
            {
                var sql = @"
                    SELECT ISNULL(SUM(
                        CASE 
                            WHEN it.TypeCategory = 'ADD' THEN it.Quantity 
                            WHEN it.TypeCategory = 'REMOVE' THEN -it.Quantity 
                            ELSE 0 
                        END), 0)
                    FROM [Transactions].[InventoryTransactions] it
                    INNER JOIN [Instances].[ProductAttributes] pa 
                        ON it.ProductInstanceId = pa.InstanceId
                    WHERE pa.[Key] = @Key AND pa.Value = @Value";

                return await conn.QuerySingleAsync<decimal>(
                    sql, new { Key = key, Value = value }, trans);
            });
        }

        // EVAL: Undo elimina la transacción para poder revertir movimientos de inventario
        public async Task UndoTransactionAsync(int transactionId)
        {
            await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    DELETE FROM [Transactions].[InventoryTransactions] 
                    WHERE TransactionId = @TransactionId";

                await conn.ExecuteAsync(sql, new { TransactionId = transactionId }, trans);
            });
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync(int productInstanceId)
        {
            return await _executor.ExecuteAsync<IEnumerable<InventoryTransaction>>(async (conn, trans) =>
            {
                var sql = @"
                    SELECT * FROM [Transactions].[InventoryTransactions]
                    WHERE ProductInstanceId = @ProductInstanceId
                    ORDER BY StartedTimestamp DESC";

                return await conn.QueryAsync<InventoryTransaction>(
                    sql, new { ProductInstanceId = productInstanceId }, trans);
            });
        }
    }
}
