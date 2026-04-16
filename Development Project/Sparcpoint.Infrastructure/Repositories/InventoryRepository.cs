using Dapper;
using Sparcpoint.Application.Interfaces;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public InventoryRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task InsertTransaction(int productId, int quantity, string type)
        {
            await _sqlExecutor.ExecuteAsync(async (conn, tx) =>
            {
                var sql = @"
                INSERT INTO transactions.InventoryTransactions
                (productinstanceid, quantity, startedtimestamp, completedtimestamp, typecategory)
                VALUES (@ProductId, @Quantity, GETUTCDATE(), GETUTCDATE(), @Type);
            ";

                await conn.ExecuteAsync(sql, new
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Type = type
                }, tx);

                
            });
        }

        public async Task<int> GetStock(int productId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, tx) =>
            {
                var sql = @"
                SELECT ISNULL(SUM(quantity), 0)
                FROM transactions.InventoryTransactions
                WHERE productinstanceid = @ProductId;
            ";

                var result = await conn.ExecuteScalarAsync<int>(sql, new
                {
                    ProductId = productId
                }, tx);

                return result;
            });
        }

        public async Task DeleteTransaction(int transactionId)
        {
            await _sqlExecutor.ExecuteAsync(async (conn, tx) =>
            {
                var sql = @"
                DELETE FROM transactions.InventoryTransactions
                WHERE transactionid = @Id;
            ";

                await conn.ExecuteAsync(sql, new { Id = transactionId }, tx);
            });
        }
    }
}
