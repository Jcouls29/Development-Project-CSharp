using Dapper;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Sparcpoint.Implementations.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;

        public InventoryRepository(ISqlExecutor executor)
        {
            _executor = executor;
        }

        public async Task<int> AddProductToInventoryAsync(AddToInventoryRequestDto request)
        {
            const string addprodsql = @"
                    INSERT INTO [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp,TypeCategory)
                    VALUES (@ProductInstanceID, @Quantity, @StartedTimestamp, @CompletedTimestamp,@TypeCategory);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _executor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                var parameters = new
                {
                    request.ProductInstanceId,
                    request.Quantity,
                    request.StartedTimestamp,
                    request.CompletedTimestamp,
                    request.TypeCategory
                };
                return await connection.ExecuteScalarAsync<int>(addprodsql, parameters, transaction);
            });
        }

        //EVAL: Delete transation record for a given transaction id, this will be used to remove the inventory record for a specific transaction, this is a soft delete and can be extended in the future to support hard delete or archiving of old transactions based on requirement
        public async Task<int> RemoveInventoryTransactionAsync(int transactionId)
        {
            string sql = "DELETE FROM [Transactions].[InventoryTransactions] WHERE TransactionID = @TransactionID";
            return await _executor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                return await connection.ExecuteAsync(sql, new { TransactionID = transactionId }, transaction);
            });
        }

        //EVAL: Delete all inventory records for a given product id, this will be used to remove the inventory record for a specific product, this is a soft delete and can be extended in the future to support hard delete or archiving of old transactions based on requirement
        public async Task<int> RemoveProductFromInventoryAsync(int productId)
        {
            string sql = "DELETE FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ProductInstanceId";

            return await _executor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                return await connection.ExecuteAsync(sql, new { ProductInstanceId = productId }, transaction);
            });
        }

        public async Task<decimal> GetProuctInventoryCountAsync(int productId)
        {
            const string sql = @"
                               SELECT COALESCE(SUM(Quantity), 0.0) 
                               FROM [Transactions].[InventoryTransactions] 
                               WHERE ProductInstanceId = @ProductId;";
            return await _executor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                var parameters = new
                {
                    ProductId = productId,
                    CurrentTime = DateTime.UtcNow
                };
                return await connection.ExecuteScalarAsync<int>(sql, parameters, transaction);
            });
        }
    }
}
