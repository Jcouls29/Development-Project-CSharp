using Dapper;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public InventoryRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public Task<int> AddTransactionAsync(InventoryTransactionRequest request)
        {
            return _sqlExecutor.ExecuteAsync((connection, transaction) =>
                connection.ExecuteScalarAsync<int>(@"
INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
OUTPUT INSERTED.[TransactionId]
VALUES (@ProductInstanceId, @Quantity, GETUTCDATE(), NULL, @TypeCategory);",
                    new
                    {
                        request.ProductInstanceId,
                        request.Quantity,
                        request.TypeCategory,
                    }, transaction));
        }

        public Task<InventoryTransactionModel> GetTransactionByIdAsync(int transactionId)
        {
            return _sqlExecutor.ExecuteAsync((connection, transaction) =>
                connection.QuerySingleOrDefaultAsync<InventoryTransactionModel>(@"
SELECT
    it.[TransactionId],
    it.[ProductInstanceId],
    it.[Quantity],
    it.[StartedTimestamp],
    it.[CompletedTimestamp],
    it.[TypeCategory]
FROM [Transactions].[InventoryTransactions] it
WHERE it.[TransactionId] = @TransactionId;",
                    new { TransactionId = transactionId }, transaction));
        }

        public Task<InventoryCountModel> GetTotalCountByProductAsync(int productInstanceId)
        {
            return _sqlExecutor.ExecuteAsync((connection, transaction) =>
                connection.QuerySingleAsync<InventoryCountModel>(@"
SELECT
    @ProductInstanceId AS [ProductInstanceId],
    COALESCE(SUM(it.[Quantity]), 0) AS [Quantity]
FROM [Transactions].[InventoryTransactions] it
WHERE it.[ProductInstanceId] = @ProductInstanceId
  AND it.[CompletedTimestamp] IS NULL;",
                    new { ProductInstanceId = productInstanceId }, transaction));
        }

        public Task<List<int>> AddBulkTransactionsAsync(List<InventoryTransactionRequest> items)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var transactionIds = new List<int>();
                foreach (var item in items)
                {
                    var id = await connection.ExecuteScalarAsync<int>(@"
INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
OUTPUT INSERTED.[TransactionId]
VALUES (@ProductInstanceId, @Quantity, GETUTCDATE(), NULL, @TypeCategory);",
                        new
                        {
                            item.ProductInstanceId,
                            item.Quantity,
                            item.TypeCategory,
                        }, transaction);
                    transactionIds.Add(id);
                }
                return transactionIds;
            });
        }

        public Task CompleteTransactionAsync(int transactionId)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                await connection.ExecuteAsync(@"
UPDATE [Transactions].[InventoryTransactions]
SET [CompletedTimestamp] = GETUTCDATE()
WHERE [TransactionId] = @TransactionId
  AND [CompletedTimestamp] IS NULL;",
                    new { TransactionId = transactionId }, transaction);

                return 0;
            });
        }

        public Task<List<InventoryCountModel>> GetCountByMetadataAsync(Dictionary<string, string> attributes)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var sql = @"
SELECT t.[ProductInstanceId], COALESCE(SUM(t.[Quantity]), 0) AS [Quantity]
FROM [Transactions].[InventoryTransactions] t
WHERE t.[CompletedTimestamp] IS NULL";

                var parameters = new DynamicParameters();
                int index = 0;
                foreach (var attr in attributes)
                {
                    var keyParam = $"@Key{index}";
                    var valueParam = $"@Value{index}";
                    sql += $@"
  AND t.[ProductInstanceId] IN (
    SELECT pa.[InstanceId]
    FROM [Instances].[ProductAttributes] pa
    WHERE pa.[Key] = {keyParam} AND pa.[Value] LIKE '%' + {valueParam} + '%'
  )";
                    parameters.Add($"Key{index}", attr.Key);
                    parameters.Add($"Value{index}", attr.Value);
                    index++;
                }

                sql += " GROUP BY t.[ProductInstanceId]";

                return (await connection.QueryAsync<InventoryCountModel>(sql, parameters, transaction)).ToList();
            });
        }

        public Task<PaginatedResult<InventoryTransactionModel>> GetTransactionsByProductAsync(int productInstanceId, int page, int pageSize)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var offset = (page - 1) * pageSize;

                var totalCount = await connection.ExecuteScalarAsync<int>(@"
SELECT COUNT(*)
FROM [Transactions].[InventoryTransactions]
WHERE [ProductInstanceId] = @ProductInstanceId
  AND [CompletedTimestamp] IS NULL;",
                    new { ProductInstanceId = productInstanceId }, transaction);

                var items = (await connection.QueryAsync<InventoryTransactionModel>(@"
SELECT
    [TransactionId],
    [ProductInstanceId],
    [Quantity],
    [StartedTimestamp],
    [CompletedTimestamp],
    [TypeCategory]
FROM [Transactions].[InventoryTransactions]
WHERE [ProductInstanceId] = @ProductInstanceId
  AND [CompletedTimestamp] IS NULL
ORDER BY [StartedTimestamp] DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;",
                    new { ProductInstanceId = productInstanceId, Offset = offset, PageSize = pageSize }, transaction)).ToList();

                return new PaginatedResult<InventoryTransactionModel>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                };
            });
        }
    }
}
