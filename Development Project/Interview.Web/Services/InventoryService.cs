using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Interview.Web.Models;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Web.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ISqlExecutor _executor;

        public InventoryService(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> CreateProductAsync(CreateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Product Name is required.");

            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                // 1. Insert Product
                string insertProductSql = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus)
                    OUTPUT INSERTED.InstanceId
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);";
                
                int instanceId = await conn.ExecuteScalarAsync<int>(insertProductSql, new
                {
                    request.Name,
                    Description = request.Description ?? string.Empty,
                    ProductImageUris = request.ProductImageUris ?? string.Empty,
                    ValidSkus = request.ValidSkus ?? string.Empty
                }, trans);

                // 2. Insert Attributes (metadata)
                if (request.Attributes != null && request.Attributes.Count > 0)
                {
                    string insertAttrSql = @"
                        INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value])
                        VALUES (@InstanceId, @Key, @Value);";

                    var attrData = request.Attributes.Select(kv => new 
                    { 
                        InstanceId = instanceId, 
                        Key = kv.Key, 
                        Value = kv.Value 
                    });

                    await conn.ExecuteAsync(insertAttrSql, attrData, trans);
                }

                return instanceId;
            });
        }

        public async Task<decimal> GetInventoryCountAsync(int productInstanceId)
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                string sql = "SELECT ISNULL(SUM(Quantity), 0) FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ProductInstanceId";
                return await conn.ExecuteScalarAsync<decimal>(sql, new { ProductInstanceId = productInstanceId }, trans);
            });
        }

        public async Task<decimal> GetInventoryCountBySearchAsync(ProductSearchRequest request)
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var queryProvider = new SqlServerQueryProvider()
                    .SetTargetTableAlias("p");

                string sql = @"
                    SELECT ISNULL(SUM(t.Quantity), 0)
                    FROM [Instances].[Products] p
                    LEFT JOIN [Transactions].[InventoryTransactions] t ON p.InstanceId = t.ProductInstanceId ";

                if (request != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.Name))
                    {
                        queryProvider.Where("p.Name LIKE @SearchName");
                        queryProvider.AddParameter("@SearchName", $"%{request.Name}%");
                    }

                    if (!string.IsNullOrWhiteSpace(request.Description))
                    {
                        queryProvider.Where("p.Description LIKE @SearchDesc");
                        queryProvider.AddParameter("@SearchDesc", $"%{request.Description}%");
                    }

                    if (!string.IsNullOrWhiteSpace(request.ValidSkus))
                    {
                        queryProvider.Where("p.ValidSkus LIKE @SearchSkus");
                        queryProvider.AddParameter("@SearchSkus", $"%{request.ValidSkus}%");
                    }
                }

                int attrCount = 0;
                if (request?.Attributes != null && request.Attributes.Count > 0)
                {
                    foreach (var filter in request.Attributes)
                    {
                        string alias = $"pa{attrCount}";
                        queryProvider.Join($"[Instances].[ProductAttributes]", alias, $"p.InstanceId = {alias}.InstanceId");
                        
                        string keyParam = $"@key{attrCount}";
                        string valParam = $"@val{attrCount}";
                        
                        queryProvider.Where($"[{alias}].[Key] = {keyParam}");
                        queryProvider.AddParameter(keyParam, filter.Key);
                        
                        queryProvider.Where($"[{alias}].[Value] = {valParam}");
                        queryProvider.AddParameter(valParam, filter.Value);
                        
                        attrCount++;
                    }
                }

                sql += queryProvider.JoinClause + " " + queryProvider.WhereClause;

                return await conn.ExecuteScalarAsync<decimal>(sql, queryProvider.Parameters, trans);
            });
        }

        public async Task<IEnumerable<InventoryTransactionDto>> GetInventoryTransactionsAsync(int productInstanceId)
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                string sql = @"
                    SELECT TransactionId, ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory 
                    FROM [Transactions].[InventoryTransactions] 
                    WHERE ProductInstanceId = @ProductInstanceId
                    ORDER BY StartedTimestamp DESC";
                    
                var results = await conn.QueryAsync<InventoryTransactionDto>(sql, new { ProductInstanceId = productInstanceId }, trans);
                return results;
            });
        }

        public async Task ProcessInventoryTransactionsAsync(List<InventoryTransactionRequest> requests)
        {
            if (requests == null || requests.Count == 0) return;

            await _executor.ExecuteAsync(async (conn, trans) =>
            {
                string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions] 
                    (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory)
                    VALUES 
                    (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), SYSUTCDATETIME(), @TypeCategory);";

                foreach(var req in requests)
                {
                    await conn.ExecuteAsync(sql, req, trans);
                }
            });
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(ProductSearchRequest request)
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var queryProvider = new SqlServerQueryProvider()
                    .SetTargetTableAlias("p");

                string sql = @"
                    SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus 
                    FROM [Instances].[Products] p ";

                if (request != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.Name))
                    {
                        queryProvider.Where("p.Name LIKE @SearchName");
                        queryProvider.AddParameter("@SearchName", $"%{request.Name}%");
                    }

                    if (!string.IsNullOrWhiteSpace(request.Description))
                    {
                        queryProvider.Where("p.Description LIKE @SearchDesc");
                        queryProvider.AddParameter("@SearchDesc", $"%{request.Description}%");
                    }

                    if (!string.IsNullOrWhiteSpace(request.ValidSkus))
                    {
                        queryProvider.Where("p.ValidSkus LIKE @SearchSkus");
                        queryProvider.AddParameter("@SearchSkus", $"%{request.ValidSkus}%");
                    }
                }

                int attrCount = 0;
                if (request?.Attributes != null && request.Attributes.Count > 0)
                {
                    foreach (var filter in request.Attributes)
                    {
                        string alias = $"pa{attrCount}";
                        queryProvider.Join($"[Instances].[ProductAttributes]", alias, $"p.InstanceId = {alias}.InstanceId");
                        
                        string keyParam = $"@key{attrCount}";
                        string valParam = $"@val{attrCount}";
                        
                        queryProvider.Where($"[{alias}].[Key] = {keyParam}");
                        queryProvider.AddParameter(keyParam, filter.Key);
                        
                        queryProvider.Where($"[{alias}].[Value] = {valParam}");
                        queryProvider.AddParameter(valParam, filter.Value);
                        
                        attrCount++;
                    }

                    sql += queryProvider.JoinClause + " " + queryProvider.WhereClause;
                }

                var products = await conn.QueryAsync<ProductDto>(sql, queryProvider.Parameters, trans);
                var productList = products.ToList();

                if (productList.Any())
                {
                    var instanceIds = productList.Select(p => p.InstanceId).ToList();
                    string attrSql = "SELECT InstanceId, [Key], [Value] FROM [Instances].[ProductAttributes] WHERE InstanceId IN @InstanceIds";
                    var attributes = await conn.QueryAsync(attrSql, new { InstanceIds = instanceIds }, trans);

                    var attrLookup = attributes
                        .GroupBy(a => (int)a.InstanceId)
                        .ToDictionary(g => g.Key, g => g.ToDictionary(a => (string)a.Key, a => (string)a.Value));

                    foreach (var p in productList)
                    {
                        if (attrLookup.TryGetValue(p.InstanceId, out var attrs))
                        {
                            p.Attributes = attrs;
                        }
                    }
                }

                return productList;
            });
        }

        public async Task UndoInventoryTransactionAsync(int productInstanceId, int originalTransactionId)
        {
            await _executor.ExecuteAsync(async (conn, trans) =>
            {
                string countSql = "SELECT ISNULL(SUM(Quantity), 0) FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ProductInstanceId";
                decimal currentInventory = await conn.ExecuteScalarAsync<decimal>(countSql, new { ProductInstanceId = productInstanceId }, trans);

                string getTxSql = "SELECT Quantity FROM [Transactions].[InventoryTransactions] WHERE TransactionId = @TransactionId AND ProductInstanceId = @ProductInstanceId";
                decimal? originalQuantity = await conn.QuerySingleOrDefaultAsync<decimal?>(getTxSql, new { TransactionId = originalTransactionId, ProductInstanceId = productInstanceId }, trans);

                if (!originalQuantity.HasValue)
                    throw new Exception("Original transaction not found.");

                decimal newQuantity = -originalQuantity.Value; // Undo

                if (currentInventory + newQuantity < 0)
                {
                    throw new InvalidOperationException("Undoing this transaction would result in negative inventory, which is not allowed.");
                }

                string insertSql = @"
                    INSERT INTO [Transactions].[InventoryTransactions] 
                    (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory)
                    VALUES 
                    (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), SYSUTCDATETIME(), 'Undo');";

                await conn.ExecuteAsync(insertSql, new { ProductInstanceId = productInstanceId, Quantity = newQuantity }, trans);
            });
        }
    }
}
