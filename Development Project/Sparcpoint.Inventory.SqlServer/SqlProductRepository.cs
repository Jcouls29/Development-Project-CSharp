using Dapper;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    /// <summary>
    /// EVAL: Dapper is chosen over EF Core deliberately — the schema uses EAV (ProductAttributes)
    /// and dynamic WHERE clause composition that benefit from explicit SQL control.
    /// Dapper gives full visibility into every query sent to the database.
    /// </summary>
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlProductRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(CreateProductRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));
            PreConditions.StringNotNullOrWhitespace(request.Description, nameof(request.Description));

            return await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                // Insert the base product record
                var productId = await conn.ExecuteScalarAsync<int>(@"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new
                    {
                        request.Name,
                        request.Description,
                        ProductImageUris = request.ProductImageUris ?? string.Empty,
                        ValidSkus = request.ValidSkus ?? string.Empty
                    }, tx);

                // Insert arbitrary metadata attributes
                // EVAL: Iterated inserts here are acceptable for write operations.
                // For bulk product imports, a TVP (Table-Valued Parameter) would be more efficient.
                if (request.Attributes != null)
                {
                    foreach (var attr in request.Attributes)
                    {
                        await conn.ExecuteAsync(@"
                            INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                            VALUES (@InstanceId, @Key, @Value)",
                            new { InstanceId = productId, attr.Key, attr.Value }, tx);
                    }
                }

                // Link product to categories
                if (request.CategoryIds != null)
                {
                    foreach (var categoryId in request.CategoryIds)
                    {
                        await conn.ExecuteAsync(@"
                            INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                            VALUES (@InstanceId, @CategoryInstanceId)",
                            new { InstanceId = productId, CategoryInstanceId = categoryId }, tx);
                    }
                }

                return productId;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchFilter filter)
        {
            return await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                var sql = new StringBuilder(@"
                    SELECT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p
                    WHERE 1=1");

                var parameters = new DynamicParameters();

                // Name filter: partial match
                if (!string.IsNullOrWhiteSpace(filter?.Name))
                {
                    sql.Append(" AND p.[Name] LIKE @Name");
                    parameters.Add("Name", $"%{filter.Name}%");
                }

                // EVAL: Each attribute filter becomes an EXISTS subquery rather than a JOIN.
                // Reason: a JOIN on EAV tables causes row multiplication — one product with
                // N attributes matching M filter criteria would produce N*M rows before DISTINCT.
                // EXISTS is cleaner and more predictable in its query plan.
                if (filter?.Attributes != null)
                {
                    int attrIndex = 0;
                    foreach (var attr in filter.Attributes)
                    {
                        var keyParam = $"AttrKey{attrIndex}";
                        var valParam = $"AttrVal{attrIndex}";
                        sql.Append($@" AND EXISTS (
                            SELECT 1 FROM [Instances].[ProductAttributes] pa{attrIndex}
                            WHERE pa{attrIndex}.[InstanceId] = p.[InstanceId]
                              AND pa{attrIndex}.[Key] = @{keyParam}
                              AND pa{attrIndex}.[Value] = @{valParam})");
                        parameters.Add(keyParam, attr.Key);
                        parameters.Add(valParam, attr.Value);
                        attrIndex++;
                    }
                }

                // Category filter: product must belong to at least one of the specified categories
                if (filter?.CategoryIds != null && filter.CategoryIds.Any())
                {
                    sql.Append(@" AND EXISTS (
                        SELECT 1 FROM [Instances].[ProductCategories] pc
                        WHERE pc.[InstanceId] = p.[InstanceId]
                          AND pc.[CategoryInstanceId] IN @CategoryIds)");
                    parameters.Add("CategoryIds", filter.CategoryIds);
                }

                var products = (await conn.QueryAsync<Product>(sql.ToString(), parameters, tx)).ToList();

                // Load attributes and category links for all returned products in two bulk queries
                // to avoid N+1 queries
                if (products.Any())
                {
                    var ids = products.Select(p => p.InstanceId).ToArray();

                    var attributes = await conn.QueryAsync<(int InstanceId, string Key, string Value)>(
                        "SELECT [InstanceId], [Key], [Value] FROM [Instances].[ProductAttributes] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, tx);

                    var categoryLinks = await conn.QueryAsync<(int InstanceId, int CategoryInstanceId)>(
                        "SELECT [InstanceId], [CategoryInstanceId] FROM [Instances].[ProductCategories] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, tx);

                    var attrLookup = attributes
                        .GroupBy(a => a.InstanceId)
                        .ToDictionary(g => g.Key, g => g.Select(a => new ProductAttribute { Key = a.Key, Value = a.Value }).ToList());

                    var catLookup = categoryLinks
                        .GroupBy(c => c.InstanceId)
                        .ToDictionary(g => g.Key, g => g.Select(c => c.CategoryInstanceId).ToList());

                    foreach (var product in products)
                    {
                        product.Attributes = attrLookup.TryGetValue(product.InstanceId, out var attrs)
                            ? attrs : new List<ProductAttribute>();
                        product.CategoryIds = catLookup.TryGetValue(product.InstanceId, out var cats)
                            ? cats : new List<int>();
                    }
                }

                return products;
            });
        }
    }
}
