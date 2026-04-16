using Dapper;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastucture
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public InventoryRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task SaveTransactionAsync(List<InventoryUpdate> updates)
        {
            // 1. Prepare the DataTable for SQL Type
            var table = new DataTable();
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("InstanceId", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));

            for (int i = 0; i < updates.Count; i++)
            {
                table.Rows.Add(i, updates[i].ProductInstanceId, string.Empty, updates[i].Quantity.ToString());
            }

            // 2. Executes one query to process all the list
            const string sql = @"
                INSERT INTO [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity)
                SELECT [InstanceId], CAST([Description] AS INT) 
                FROM @BulkList;";

            await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                await conn.ExecuteAsync(sql, new
                {
                    BulkList = table.AsTableValuedParameter("[Instances].[CorrelatedListItemList]")
                }, trans);
            });
        }

        public async Task UndoTransactionAsync(Guid transactionId)
        {
            // EVAL: Validate pre-conditions using the class provided in the Core
            PreConditions.ParameterNotNull(transactionId, nameof(transactionId));

            const string sqlUndoTransaction = @"UPDATE [Transactions].[InventoryTransactions] SET IsUndone = 1 WHERE TransactionId = @transactionId;";

            await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                await connection.ExecuteAsync(sqlUndoTransaction, new { transactionId }, transaction);
            });
        }

        public async Task<int> GetStockCountAsync(int productInstanceId)
        {
            // EVAL: Validate pre-conditions using the class provided in the Core
            PreConditions.ParameterNotNull(productInstanceId, nameof(productInstanceId));

            const string sqlGetStockCount = @"
                    SELECT ISNULL(SUM(Quantity), 0) FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @productInstanceId AND IsUndone = 0;";

            return await _sqlExecutor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                return await connection.ExecuteScalarAsync<int>(sqlGetStockCount, new { productInstanceId }, transaction);
            });
        }
    }
}