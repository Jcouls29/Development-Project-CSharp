using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;
using Sparcpoint.Inventory.Repositories.Interfaces;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.Repositories.Implementations
{
    /// <summary>
    /// Inventory transaction repository implementation.
    /// EVAL: Handles bulk inventory operations and supports transaction undo functionality.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;
        private readonly ILogger<InventoryRepository> _logger;

        public InventoryRepository(ISqlExecutor sqlExecutor, ILogger<InventoryRepository> logger)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<InventoryTransaction>> AddInventoryAsync(List<InventoryTransaction> transactions)
        {
            // EVAL: Bulk insert using OUTPUT clause to return generated IDs
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var createdTransactions = new List<InventoryTransaction>();

                // EVAL: Using individual inserts with OUTPUT for transaction ID capture
                // Could be optimized with bulk TVP insert if transaction volume is very high
                foreach (var transaction in transactions)
                {
                    const string sql = @"
                        INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
                        OUTPUT INSERTED.*
                        VALUES
                        (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory)";

                    var created = await conn.QuerySingleAsync<InventoryTransactionDto>(
                        sql,
                        new
                        {
                            transaction.ProductInstanceId,
                            transaction.Quantity,
                            StartedTimestamp = transaction.StartedTimestamp == default ? DateTime.UtcNow : transaction.StartedTimestamp,
                            CompletedTimestamp = transaction.CompletedTimestamp ?? DateTime.UtcNow, // Auto-complete by default
                            transaction.TypeCategory
                        },
                        trans);

                    createdTransactions.Add(new InventoryTransaction
                    {
                        TransactionId = created.TransactionId,
                        ProductInstanceId = created.ProductInstanceId,
                        Quantity = created.Quantity,
                        StartedTimestamp = created.StartedTimestamp,
                        CompletedTimestamp = created.CompletedTimestamp,
                        TypeCategory = created.TypeCategory
                    });
                }

                _logger.LogInformation("Created {Count} inventory transactions", createdTransactions.Count);
                return createdTransactions;
            });
        }

        public async Task<bool> RemoveTransactionAsync(int transactionId)
        {
            // EVAL: Supports requirement for ability to undo transactions
            // Physical delete rather than soft delete for true undo capability
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = @"
                    DELETE FROM [Transactions].[InventoryTransactions]
                    WHERE [TransactionId] = @TransactionId";

                var rowsAffected = await conn.ExecuteAsync(sql, new { TransactionId = transactionId }, trans);
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Removed inventory transaction {TransactionId}", transactionId);
                    return true;
                }

                _logger.LogWarning("Transaction {TransactionId} not found for removal", transactionId);
                return false;
            });
        }

        public async Task<List<InventoryCountResponse>> GetInventoryCountAsync(InventoryCountCriteria criteria)
        {
            // EVAL: Supports requirement for retrieving counts by product ID or metadata
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sqlBuilder = new System.Text.StringBuilder();
                var parameters = new DynamicParameters();

                sqlBuilder.AppendLine(@"
                    SELECT 
                        p.[InstanceId] AS ProductInstanceId,
                        p.[Name] AS ProductName,
                        SUM(it.[Quantity]) AS TotalQuantity,
                        COUNT(it.[TransactionId]) AS TransactionCount
                    FROM [Instances].[Products] p
                    INNER JOIN [Transactions].[InventoryTransactions] it ON p.[InstanceId] = it.[ProductInstanceId]");

                var whereClauses = new List<string>();

                // Filter by specific product
                if (criteria.ProductInstanceId.HasValue)
                {
                    whereClauses.Add("p.[InstanceId] = @ProductInstanceId");
                    parameters.Add("ProductInstanceId", criteria.ProductInstanceId.Value);
                }

                // Filter by completed transactions only
                if (criteria.OnlyCompleted)
                {
                    whereClauses.Add("it.[CompletedTimestamp] IS NOT NULL");
                }

                // Filter by product attributes
                if (criteria.ProductAttributes != null && criteria.ProductAttributes.Any())
                {
                    var attrIndex = 0;
                    foreach (var attr in criteria.ProductAttributes)
                    {
                        sqlBuilder.AppendLine($@"
                            INNER JOIN [Instances].[ProductAttributes] pa{attrIndex}
                                ON p.[InstanceId] = pa{attrIndex}.[InstanceId]
                                AND pa{attrIndex}.[Key] = @AttrKey{attrIndex}
                                AND pa{attrIndex}.[Value] = @AttrValue{attrIndex}");
                        
                        parameters.Add($"AttrKey{attrIndex}", attr.Key);
                        parameters.Add($"AttrValue{attrIndex}", attr.Value);
                        attrIndex++;
                    }
                }

                // Filter by categories
                if (criteria.CategoryIds != null && criteria.CategoryIds.Any())
                {
                    var catIndex = 0;
                    foreach (var categoryId in criteria.CategoryIds)
                    {
                        sqlBuilder.AppendLine($@"
                            INNER JOIN [Instances].[ProductCategories] pc{catIndex}
                                ON p.[InstanceId] = pc{catIndex}.[InstanceId]
                                AND pc{catIndex}.[CategoryInstanceId] = @CategoryId{catIndex}");
                        
                        parameters.Add($"CategoryId{catIndex}", categoryId);
                        catIndex++;
                    }
                }

                if (whereClauses.Any())
                {
                    sqlBuilder.AppendLine("WHERE " + string.Join(" AND ", whereClauses));
                }

                sqlBuilder.AppendLine(@"
                    GROUP BY p.[InstanceId], p.[Name]
                    ORDER BY p.[Name]");

                var sql = sqlBuilder.ToString();
                _logger.LogDebug("Executing inventory count query: {Sql}", sql);

                var results = await conn.QueryAsync<InventoryCountResponse>(sql, parameters, trans);
                return results.ToList();
            });
        }

        public async Task<InventoryTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT 
                        it.*,
                        p.[Name] AS ProductName
                    FROM [Transactions].[InventoryTransactions] it
                    INNER JOIN [Instances].[Products] p ON it.[ProductInstanceId] = p.[InstanceId]
                    WHERE it.[TransactionId] = @TransactionId";

                var dto = await conn.QuerySingleOrDefaultAsync<InventoryTransactionDto>(
                    sql,
                    new { TransactionId = transactionId },
                    trans);

                if (dto == null) return null;

                return new InventoryTransaction
                {
                    TransactionId = dto.TransactionId,
                    ProductInstanceId = dto.ProductInstanceId,
                    Quantity = dto.Quantity,
                    StartedTimestamp = dto.StartedTimestamp,
                    CompletedTimestamp = dto.CompletedTimestamp,
                    TypeCategory = dto.TypeCategory,
                    ProductName = dto.ProductName
                };
            });
        }

        public async Task<List<InventoryTransaction>> GetTransactionsByProductIdAsync(int productInstanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT 
                        it.*,
                        p.[Name] AS ProductName
                    FROM [Transactions].[InventoryTransactions] it
                    INNER JOIN [Instances].[Products] p ON it.[ProductInstanceId] = p.[InstanceId]
                    WHERE it.[ProductInstanceId] = @ProductInstanceId
                    ORDER BY it.[StartedTimestamp] DESC";

                var dtos = await conn.QueryAsync<InventoryTransactionDto>(
                    sql,
                    new { ProductInstanceId = productInstanceId },
                    trans);

                return dtos.Select(dto => new InventoryTransaction
                {
                    TransactionId = dto.TransactionId,
                    ProductInstanceId = dto.ProductInstanceId,
                    Quantity = dto.Quantity,
                    StartedTimestamp = dto.StartedTimestamp,
                    CompletedTimestamp = dto.CompletedTimestamp,
                    TypeCategory = dto.TypeCategory,
                    ProductName = dto.ProductName
                }).ToList();
            });
        }

        #region DTOs

        private class InventoryTransactionDto
        {
            public int TransactionId { get; set; }
            public int ProductInstanceId { get; set; }
            public decimal Quantity { get; set; }
            public DateTime StartedTimestamp { get; set; }
            public DateTime? CompletedTimestamp { get; set; }
            public string? TypeCategory { get; set; }
            public string? ProductName { get; set; }
        }

        #endregion
    }
}